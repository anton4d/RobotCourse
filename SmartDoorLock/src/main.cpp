#include <Arduino.h>
#include <ESP32Servo.h>
#include <WiFi.h>
#include <WiFiUdp.h>

hw_timer_t *timer = NULL;

/* WiFi */
// Network name (SSID) and password (WPA)
constexpr char SSID_NAME[] = "Raspi";
constexpr char SSID_PASSWORD[] = "Fordgt40";

/* UDP */
// Udp object
WiFiUDP Udp;

// Receiver IP-address and port
IPAddress RECEIVER_IP_ADDRESS(192, 168, 131, 147);
constexpr int RECEIVER_PORT = 50195;
constexpr int LOCAL_PORT = 3002;

// Data string used to send UDP messages
String UDPDataString = "";

// Char array used to receive UDP messages (assuming max packet size is 255 bytes)
char UDPPacketBuffer[255];

// create servo object to control a servo
Servo myservo;

// Used pins
constexpr byte POTENTIOMETER1_PIN = 35;
constexpr byte POTENTIOMETER_PIN = 34;
constexpr byte Servo_PIN = 12;

// Define the pins for the buttons
uint8_t buttonPins[9] = {14, 15, 32, 33, 25, 26, 27, 23, 19};

// Define the pin for LEDs

const int yellowLEDPin = 4;
const int greenLEDPin = 5;
const int redLEDPin = 18;

// Define the combination
const int correctCombination[4] = {5, 5, 5, 5};
int enteredCombination[4] = {0, 0, 0, 0};
int currentIndex = 0;

// Define the state for indicating success or failure
bool success = false;

// Lock bools
bool LastLockState = false;

// door bools
bool LastDoorState;
bool DoorIsOpen = false;
bool LastDoorHandleState;

// timer Duration in secounds
int timerDuration = 10;

// Function prototype
void LockFunction(bool);
void TimerDebug();
void sendUDPDataString();
void receiveUDPMessage();

// Declare the Task Functions
void TaskDoor(void *pvParameters);
void TaskDoorHandle(void *pvParameters);
void TaskUnity(void **pvParameters);

// timerBools
bool TimerStop = false;

// Timer inturupt
void IRAM_ATTR onTimer()
{
  TimerStop = true;
}

void setup()
{
  // enable Serial
  Serial.begin(9600);
  while (!Serial)
    ;
  /* WiFi */
  // Begin WiFi
  WiFi.begin(SSID_NAME, SSID_PASSWORD);
  while (WiFi.status() != WL_CONNECTED)
  {
    Serial.print("Attempting to connect to SSID: ");
    Serial.println(SSID_NAME);

    delay(1000);
  }
  Serial.println("Connected to WiFi");

  /* UDP */
  // Begin UDP
  Udp.begin(LOCAL_PORT);
  Serial.println("UDP Begun");

  // Timer Setup
  timer = timerBegin(0, 80, true);
  timerAttachInterrupt(timer, &onTimer, true);

  // Setup Door task
  xTaskCreate(
      TaskDoor,          // Task function
      "DoorOpenChecker", // Name of the task (for debugging purposes)
      2000,              // Stack size (bytes)
      NULL,              // Parameter to pass to the task (optional)
      1,                 // Priority of the task, ranging from 1 (least important) to 5 (most important)
      NULL);             // Task handle, used to reference the task (optional)

  // Setup DoorHandle
  xTaskCreate(
      TaskDoorHandle,
      "Doorhandle",
      1000,
      NULL,
      1,
      NULL);

  // Allow allocation of all timers
  ESP32PWM::allocateTimer(0);
  ESP32PWM::allocateTimer(1);
  ESP32PWM::allocateTimer(2);
  ESP32PWM::allocateTimer(3);

  // standard 50 hz servo
  myservo.setPeriodHertz(50);

  // attaches the servo to the used pin
  myservo.attach(Servo_PIN, 500, 2400);

  // Set button pins as inputs
  for (int i = 0; i < 9; i++)
  {
    pinMode(buttonPins[i], INPUT_PULLUP);
  }
  // set pinMode
  pinMode(POTENTIOMETER_PIN, INPUT);
  pinMode(POTENTIOMETER1_PIN, INPUT);
  pinMode(yellowLEDPin, OUTPUT);
  pinMode(greenLEDPin, OUTPUT);
  pinMode(redLEDPin, OUTPUT);
}

void loop()
{
  // Check if any button is pressed
  if (TimerStop)
  {
    TimerDebug();
    if (LastLockState == true && DoorIsOpen == false)
    {
      LockFunction(false);
    }
    TimerStop = false;
  }

  for (int i = 0; i < 9; i++)
  {
    if (digitalRead(buttonPins[i]) == LOW)
    {
      Serial.print("i is: ");
      Serial.print(i);
      Serial.print(", and buttonPin[i] is:");
      Serial.print(buttonPins[i]);
      Serial.println(", which is now LOW");

      UDPDataString = "key" + String(i) + "|1";
      sendUDPDataString();
      // Button is pressed, register the digit
      enteredCombination[currentIndex] = i + 1; // Add 1 to convert index to digit
      currentIndex++;
      // Serial.println(buttonPins[i]);
      // Serial.println(currentIndex);
      // Turn on the yellow LED momentarily
      digitalWrite(yellowLEDPin, HIGH);
      delay(300);
      digitalWrite(yellowLEDPin, LOW);
      for (int j = 0; j <= 3; j++)
      {
        Serial.println(enteredCombination[j]);
      }
      Serial.println("-----------------");

      // Wait for the button to be released
      while (digitalRead(buttonPins[i]) == LOW)
      {
        delay(200);
      }
      UDPDataString = "key" + String(i) + "|0";
      sendUDPDataString();

      // Check if the entered combination length is 4
      if (currentIndex == 4)
      {
        // Check if the combination is correct
        success = true;
        for (int j = 0; j < 4; j++)
        {
          if (enteredCombination[j] != correctCombination[j])
          {
            success = false;
            break;
          }
        }

        // Light up the appropriate LED based on the success
        if (success && LastLockState == false)
        {
          digitalWrite(greenLEDPin, HIGH);
          LockFunction(true);                                     // call the lockfuntion to upen the lock
          timerAlarmWrite(timer, timerDuration * 1000000, false); // Set the Timer amount
          timerRestart(timer);                                    // Timer Restart
          timerAlarmEnable(timer);                                // Start the timer
        }
        else if (success)
        {
          digitalWrite(yellowLEDPin, HIGH);
          delay(50);
          digitalWrite(yellowLEDPin, LOW);
          delay(50);
          digitalWrite(greenLEDPin, HIGH);
        }
        else
        {
          digitalWrite(redLEDPin, HIGH);
        }

        // Reset the combination and index for next attempt
        currentIndex = 0;
        delay(2000); // Wait for 2 seconds before resetting
        digitalWrite(greenLEDPin, LOW);
        digitalWrite(redLEDPin, LOW);
      }
    }
  }
}

void TaskDoor(void *pvParameters)
{
  int lastValue = 0;
  while (true)
  {
    // potentiometer value
    int potentiometerValue;

    potentiometerValue = analogRead(POTENTIOMETER_PIN);
    if (abs(potentiometerValue - lastValue) > 50)
    {
      UDPDataString = "potentiometer|" + String(potentiometerValue);
      sendUDPDataString();
      lastValue = potentiometerValue;
    }
    // Serial.println(potentiometerValue);
    if (potentiometerValue >= 50 && LastDoorState == true)
    {
      // Serial.println("Door is open");
    }
    else if (potentiometerValue >= 50)
    {
      DoorIsOpen = true;
      LastDoorState = true;
      Serial.println("Door has opened");
    }
    if (potentiometerValue <= 50 == LastDoorState == true)
    {
      DoorIsOpen = false;
      LastDoorState = false;
      LockFunction(false);
    }

    delay(200);
  }
}

void TaskDoorHandle(void *PvParameters)
{
  int lastValue = 0;
  while (true)
  {
    // potentiometer value
    int lastValue;
    int potentiometerValue;
    potentiometerValue = analogRead(POTENTIOMETER1_PIN);

    if (abs(potentiometerValue - lastValue) > 50)
    {
      UDPDataString = "potentiometer|" + String(potentiometerValue);
      sendUDPDataString();
      lastValue = potentiometerValue;
    }

    if (potentiometerValue >= 50 == LastDoorHandleState == true)
    {
    }
    else if (potentiometerValue >= 50)
    {
      Serial.println("DoorHandle is opened");
      LastDoorHandleState = true;
    }
    if (potentiometerValue <= 50 == LastDoorHandleState == true)
    {
      Serial.println("DoorHandle is closed");
      LastDoorHandleState = false;
    }
    delay(200);
    lastValue = potentiometerValue;
  }
}

void LockFunction(bool LockState)
{
  int pos = 0;
  if (LockState == true && LastLockState == false)
  {
    LastLockState = LockState;
    pos = 90;
    myservo.write(pos);
    Serial.println("Lock is open");
  }
  else if (LockState == false)
  {
    LastLockState = LockState;

    pos = 170;
    myservo.write(pos);
    Serial.println("Lock is closed");
  }
}

void TimerDebug()
{
  Serial.println("Timer Have Run out");
}

/* UDP */
// Send current UDPDataString to Unity

void sendUDPDataString()
{
  Udp.beginPacket(RECEIVER_IP_ADDRESS, RECEIVER_PORT);
  Udp.print(UDPDataString);
  Udp.endPacket();
  Serial.print("Send UDP message: ");
  Serial.println(UDPDataString);
}

// Receive UDP DataString from Unity
void receiveUDPMessage()
{
  if (Udp.parsePacket())
  {
    int length = Udp.read(UDPPacketBuffer, 255);
    if (length > 0)
    {
      UDPPacketBuffer[length] = 0;
      Serial.print("Received UDP message: ");
      Serial.println(UDPPacketBuffer);
    }

    // Parse the message
    char *part;
    char actuatorID[255];
    int value;

    // Get the actuator ID
    part = strtok(UDPPacketBuffer, "|");
    if (part != NULL)
    {
      strcpy(actuatorID, part);
    }

    // Get the actuator value
    part = strtok(NULL, "|");
    if (part != NULL)
    {
      value = atoi(part); // Convert string to integer
    }
  }
}