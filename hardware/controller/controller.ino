#define RED_PIN 9
#define GREEN_PIN 10
#define BLUE_PIN 11

#define BUTTON1_PIN 3  // D3 sends "1"
#define BUTTON2_PIN 2  // D2 sends "2"

void setup() {
  Serial.begin(9600);
  pinMode(RED_PIN, OUTPUT);
  pinMode(GREEN_PIN, OUTPUT);
  pinMode(BLUE_PIN, OUTPUT);

  pinMode(BUTTON1_PIN, INPUT_PULLUP);
  pinMode(BUTTON2_PIN, INPUT_PULLUP);

  turnOffLED();
  tealFade();  // Start with teal fade effect
}

void loop() {
  checkButtonPress(BUTTON1_PIN, '1');
  checkButtonPress(BUTTON2_PIN, '2');

  if (Serial.available() > 0) {
    int effectType = Serial.parseInt();
    executeAnimation(effectType);
  }
}

// Function to execute animations based on effectType
void executeAnimation(int effectType) {
  switch (effectType) {
    case 1: fastFlashingRed(); break;  // Fast Flashing Red (Red)
    case 2: cyanPurpleFade(); break;   // Cyan Purple Fade (Cyan, Purple)
    case 3: calmSafetyAmber(); break;  // Calm Safety Amber (Amber/Orange)
    case 4: glitchyTerror(); break;    // Glitchy Terror (Random Colors)
    case 5: rainbowCycle(); break;     // Rainbow Cycle (Multicolor)
    case 6: passionPulse(); break;     // Passion Pulse (Red Shades)
    case 7: melancholyWave(); break;   // Melancholy Wave (Blue, Dark Blue)
    case 8: euphoriaBurst(); break;    // Euphoria Burst (Bright Yellow)
    case 9: sereneFlow(); break;       // Serene Flow (Teal, Green)
    case 10: chaosStrobe(); break;     // Chaos Strobe (Random Strobing Colors)
    case 11: hopeGlow(); break;        // Hope Glow (Soft Yellow)
    case 12: angerFlare(); break;      // Anger Flare (Intense Red)
    case 13: joyfulSpark(); break;     // Joyful Spark (Warm Yellow, Orange)
    case 14: lonelinessGlow(); break;  // Loneliness Glow (Dark Blue, Purple)
    case 15: tealFade(); break;        // Teal Fade (New Effect)
    case 16: nostalgicGlow(); break;   // Nostalgic Glow (Sepia Tones)
    case 17: mysticalShimmer(); break; // Mystical Shimmer (Purple, Blue)
    default: turnOffLED(); break;      // Turn off for unknown commands
  }
}

// Button press handling function (consume pressed)
void checkButtonPress(int pin, char message) {
  static bool button1Pressed = false;
  static bool button2Pressed = false;

  if (pin == BUTTON1_PIN) {
    if (digitalRead(pin) == LOW && !button1Pressed) {
      Serial.println(message);
      button1Pressed = true;
    }
    if (digitalRead(pin) == HIGH) {
      button1Pressed = false;
    }
  }

  if (pin == BUTTON2_PIN) {
    if (digitalRead(pin) == LOW && !button2Pressed) {
      Serial.println(message);
      button2Pressed = true;
    }
    if (digitalRead(pin) == HIGH) {
      button2Pressed = false;
    }
  }
}

// Fast Flashing Red
void fastFlashingRed() {
  for (int i = 0; i < 10; i++) {
    setColor(255, 0, 0);
    delay(100);
    turnOffLED();
    delay(100);
  }
}

// Cyan Purple Fade
void cyanPurpleFade() {
  fadeBetweenColors(0, 255, 255, 128, 0, 128, 3000);
}

// Calm Safety Amber
void calmSafetyAmber() {
  setColor(255, 165, 0);
  delay(2500);
  turnOffLED();
}

// Glitchy Terror
void glitchyTerror() {
  for (int i = 0; i < 20; i++) {
    setColor(random(0, 255), 0, random(0, 255));
    delay(100);
  }
  turnOffLED();
}

// Rainbow Cycle
void rainbowCycle() {
  int colors[][3] = {{255,0,0}, {255,127,0}, {255,255,0}, {0,255,0}, {0,0,255}, {75,0,130}, {148,0,211}};
  for (int i = 0; i < 7; i++) {
    setColor(colors[i][0], colors[i][1], colors[i][2]);
    delay(500);
  }
}

// Passion Pulse
void passionPulse() {
  pulseColor(255, 50, 50, 2000);
}

// Melancholy Wave
void melancholyWave() {
  fadeBetweenColors(50, 50, 150, 0, 0, 100, 3000);
}

// Euphoria Burst
void euphoriaBurst() {
  for (int i = 0; i < 5; i++) {
    setColor(255, 255, 50);
    delay(300);
    turnOffLED();
    delay(300);
  }
}

// Serene Flow
void sereneFlow() {
  pulseColor(50, 255, 200, 3500);
}

// Chaos Strobe
void chaosStrobe() {
  for (int i = 15; i > 0; i--) {
    setColor(random(255), random(255), random(255));
    delay(100);
  }
  turnOffLED();
}

// Hope Glow
void hopeGlow() {
  pulseColor(255, 255, 100, 3000);
}

// Anger Flare
void angerFlare() {
  for (int i = 0; i < 5; i++) {
    setColor(255, 0, 50);
    delay(100);
    turnOffLED();
    delay(100);
  }
}

// Joyful Spark
void joyfulSpark() {
  pulseColor(255, 200, 50, 2000);
}

// Loneliness Glow
void lonelinessGlow() {
  fadeBetweenColors(100, 100, 200, 0, 0, 50, 4000);
}

// Nostalgic Glow
void nostalgicGlow() {
  pulseColor(200, 150, 50, 3500);
}

// Mystical Shimmer
void mysticalShimmer() {
  fadeBetweenColors(150, 0, 255, 75, 0, 130, 3000);
}

// Teal Fade (New Effect)
void tealFade() {
  fadeBetweenColors(0, 128, 128, 0, 255, 255, 4000);
}

// Helper Functions

void setColor(int r, int g, int b) {
  analogWrite(RED_PIN, r);
  analogWrite(GREEN_PIN, g);
  analogWrite(BLUE_PIN, b);
}

void turnOffLED() {
  setColor(0, 0, 0);
}

void pulseColor(int r, int g, int b, int duration) {
  for (int i = 0; i <= 255; i += 5) {
    setColor(r * i / 255, g * i / 255, b * i / 255);
    delay(duration / 50);
  }
  for (int i = 255; i >= 0; i -= 5) {
    setColor(r * i / 255, g * i / 255, b * i / 255);
    delay(duration / 50);
  }
}

void fadeBetweenColors(int r1, int g1, int b1, int r2, int g2, int b2, int duration) {
  for (int i = 0; i <= 255; i += 5) {
    setColor(map(i, 0, 255, r1, r2), map(i, 0, 255, g1, g2), map(i, 0, 255, b1, b2));
    delay(duration / 50);
  }
  turnOffLED();
}
