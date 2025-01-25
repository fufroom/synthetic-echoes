#define RED_PIN 9
#define GREEN_PIN 10
#define BLUE_PIN 11

#define BUTTON1_PIN 3  // D3 sends "1"
#define BUTTON2_PIN 2  // D2 sends "2"

bool isAnimationRunning = false;

// Helper function to parse incoming serial data
void processSerialCommand(String command);

// Set up serial communication and LED pins
void setup() {
  Serial.begin(9600);
  pinMode(RED_PIN, OUTPUT);
  pinMode(GREEN_PIN, OUTPUT);
  pinMode(BLUE_PIN, OUTPUT);
  
  pinMode(BUTTON1_PIN, INPUT_PULLUP);
  pinMode(BUTTON2_PIN, INPUT_PULLUP);
  
  setDefaultLED();  // Set default amber color at low brightness
}

void loop() {
  checkButtonPress();

  if (Serial.available()) {
    String receivedCommand = Serial.readStringUntil('\n');  // Read entire command
    receivedCommand.trim();
    if (receivedCommand.length() > 0) {
      processSerialCommand(receivedCommand);
    }
  }
}

// Button handling for Unity messages
void checkButtonPress() {
  static bool button1Pressed = false;
  static bool button2Pressed = false;

  if (digitalRead(BUTTON1_PIN) == LOW && !button1Pressed) {
    Serial.println("1");
    button1Pressed = true;
    delay(50);
  }
  if (digitalRead(BUTTON1_PIN) == HIGH) {
    button1Pressed = false;
  }

  if (digitalRead(BUTTON2_PIN) == LOW && !button2Pressed) {
    Serial.println("2");
    button2Pressed = true;
    delay(50);
  }
  if (digitalRead(BUTTON2_PIN) == HIGH) {
    button2Pressed = false;
  }
}

// Set default LED color
void setDefaultLED() {
  setColorRGBA(255, 212, 42, 50);  // Low alpha amber color
}

// Function to parse and process Unity's serial commands
void processSerialCommand(String command) {
  char commandBuffer[50];
  command.toCharArray(commandBuffer, 50);

  char* animationType = strtok(commandBuffer, ",");
  int r1 = atoi(strtok(NULL, ","));
  int g1 = atoi(strtok(NULL, ","));
  int b1 = atoi(strtok(NULL, ","));
  int a1 = atoi(strtok(NULL, ","));
  int r2 = atoi(strtok(NULL, ","));
  int g2 = atoi(strtok(NULL, ","));
  int b2 = atoi(strtok(NULL, ","));
  int a2 = atoi(strtok(NULL, ","));
  int duration = atoi(strtok(NULL, ","));

  if (strcmp(animationType, "fade") == 0) {
    fadeBetweenColors(r1, g1, b1, a1, r2, g2, b2, a2, duration);
  } 
  else if (strcmp(animationType, "flicker") == 0) {
    flickerColor(r1, g1, b1, a1, duration);
  }
}

// Function to fade between two colors
void fadeBetweenColors(int r1, int g1, int b1, int a1, int r2, int g2, int b2, int a2, int duration) {
  for (int i = 0; i <= 255; i += 5) {
    setColorRGBA(
      map(i, 0, 255, r1, r2),
      map(i, 0, 255, g1, g2),
      map(i, 0, 255, b1, b2),
      map(i, 0, 255, a1, a2)
    );
    delay(duration / 50);
  }
  setDefaultLED();
}

// Function to flicker a color on and off
void flickerColor(int r, int g, int b, int a, int duration) {
  for (int i = 0; i < 5; i++) {
    setColorRGBA(r, g, b, a);
    delay(duration / 10);
    setColorRGBA(0, 0, 0, 0);
    delay(duration / 10);
  }
  setDefaultLED();
}

// Helper function to set LED color with alpha
void setColorRGBA(int r, int g, int b, int a) {
  analogWrite(RED_PIN, (r * a) / 255);
  analogWrite(GREEN_PIN, (g * a) / 255);
  analogWrite(BLUE_PIN, (b * a) / 255);
}
