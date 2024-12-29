#include <Wire.h>

#define CAP1188_I2C_ADDRESS 0x29 // Default I2C address
#define REG_TOUCHED_STATUS 0x03  // Touched Status Register
#define REG_MAIN_CONTROL 0x00    // Main Control Register
#define REG_SENSITIVITY 0x1F     // Sensitivity Control Register
#define REG_C8_THRESHOLD 0x37    // Threshold Register for C8
#define REG_CALIBRATE 0x26       // Recalibration Register

#define RED_PIN 9   // Pin controlling Red
#define GREEN_PIN 10 // Pin controlling Green
#define BLUE_PIN 11  // Pin controlling Blue (not used for amber)

bool isTouched = false;          // Tracks current touch state
unsigned long lastRecalibration = 0;

void setup() {
  Serial.begin(9600);
  Wire.begin();

  // Initialize RGB LED pins
  pinMode(RED_PIN, OUTPUT);
  pinMode(GREEN_PIN, OUTPUT);
  pinMode(BLUE_PIN, OUTPUT);

  // Ensure LED is off initially
  turnOffLED();

  Serial.println("Initializing CAP1188...");

  // Verify CAP1188 presence
  if (!devicePresent()) {
    Serial.println("CAP1188 not detected. Check wiring!");
    while (true) {
      delay(1000); // Halt if device is not detected
    }
  }

  // Set sensitivity to medium
  writeRegister(REG_SENSITIVITY, 0x10); // Medium sensitivity
  Serial.println("Sensitivity set to medium.");

  // Increase threshold for C8 to avoid false positives
  writeRegister(REG_C8_THRESHOLD, 0x40); // High threshold for C8
  Serial.println("C8 threshold increased.");

  // Initial calibration
  recalibrateC8();
}

void loop() {
  uint8_t touchStatus = readRegister(REG_TOUCHED_STATUS);

  // Check only C8 (bit 7)
  if (touchStatus & (1 << 7)) {
    if (!isTouched) { // Ensure we only act on new touches
      Serial.println("Touch detected on C8.");
      setAmberColor(); // Light up LED strip in amber
      isTouched = true;
    }
  } else {
    if (isTouched) { // Ensure we only act when touch ends
      Serial.println("Touch ended on C8.");
      turnOffLED(); // Turn off the LED strip
      isTouched = false;
    }
  }

  // Periodic recalibration to handle environmental changes
  if (millis() - lastRecalibration > 5000) {
    recalibrateC8();
    lastRecalibration = millis();
  }

  // Clear touch state in the CAP1188 to ensure proper updates
  clearTouchState();

  delay(200); // Short delay for readability
}

void setAmberColor() {
  // Amber color: Combine Red and Green
  analogWrite(RED_PIN, 255);   // Maximum brightness for Red
  analogWrite(GREEN_PIN, 128); // Medium brightness for Green
  analogWrite(BLUE_PIN, 0);    // Ensure Blue is off
}

void turnOffLED() {
  // Turn off all LEDs
  analogWrite(RED_PIN, 0);
  analogWrite(GREEN_PIN, 0);
  analogWrite(BLUE_PIN, 0);
}

void recalibrateC8() {
  // Recalibrate C8 to clear touch state and adjust to environment
  writeRegister(REG_CALIBRATE, 0xFF); // Recalibrate all sensors
  Serial.println("Recalibration triggered.");
}

void clearTouchState() {
  // Clear the main control flags to reset touch state
  writeRegister(REG_MAIN_CONTROL, 0x00);
}

bool devicePresent() {
  uint8_t productID = readRegister(0xFD); // Product ID Register
  return (productID == 0x50);             // CAP1188 Product ID
}

bool writeRegister(uint8_t reg, uint8_t value) {
  Wire.beginTransmission(CAP1188_I2C_ADDRESS);
  Wire.write(reg);
  Wire.write(value);
  return (Wire.endTransmission() == 0);
}

uint8_t readRegister(uint8_t reg) {
  Wire.beginTransmission(CAP1188_I2C_ADDRESS);
  Wire.write(reg);
  if (Wire.endTransmission() != 0) {
    return 0xFF; // Return error value
  }
  Wire.requestFrom(CAP1188_I2C_ADDRESS, 1);
  return Wire.available() ? Wire.read() : 0xFF;
}
