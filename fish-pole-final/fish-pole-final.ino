/* This sketch supports National Palace Project Fishing:
 * - UNO receives message directly from PC by serial port.
 * - uses Arduino UNO and Monster Motor Shield VNH2SP30 to control 1 DC motor.
 *   need some more time to check motor spec (different RPM)
 * 
 *   20.04.18 wjie
 */

#define BRAKE 0
#define CW    1
#define CCW   2
#define CS_THRESHOLD 15   // Definition of safety current (Check: "1.3 Monster Shield Example").

//MOTOR 1
#define MOTOR_A1_PIN 7
#define MOTOR_B1_PIN 8
#define PWM_MOTOR_1 5
#define CURRENT_SEN_1 A2
#define EN_PIN_1 A0
#define MOTOR_1 0
#define MOTOR_2 1

short usSpeed = 100;  //default motor speed
unsigned short usMotor_Status = BRAKE;
String inputString = "";
bool stringComplete = false;
int movement;
int motor_speed = 0;

// distance sensor SG105
int sum = 0;
#define maxDis 600
#define minDis 300
#define INITIAL 0
#define ADDMAX 1
#define ADDMIN 2
//int distance;
//int cycles;
//int state;
int prevDis;
int curDis;

void setup()                         
{
  pinMode(MOTOR_A1_PIN, OUTPUT);
  pinMode(MOTOR_B1_PIN, OUTPUT);
  pinMode(PWM_MOTOR_1, OUTPUT);
  pinMode(CURRENT_SEN_1, OUTPUT);
  pinMode(EN_PIN_1, OUTPUT);
  prevDis = analogRead(A4);
  Serial.begin(9600); 

//  distance = 0;
//  cycles = 0;
//  state = INITIAL;
}

void loop() 
{ 
  curDis = analogRead(A4);
  int out = abs(curDis-prevDis);
  if(out > 20){
    sum += out;
    prevDis = curDis;
  }
//  Serial.println(analogRead(A4));
//  int d0 = analogRead(A4);
//  int d1 = analogRead(A4);
//  int out = abs(d1 - d0);
//  sum += out;
//  Serial.println(sum);

  if (sum > 2500) {
    Serial.println(1);
    sum = 0;    
  }
//  else
//  {
//    Serial.println(0);
//  }
  delay(500);
//  distance = 1024 - analogRead(A4);
//  Serial.println(distance);
//  if(distance > maxDis){
//    if(state == ADDMAX || state == INITIAL){
//      cycles++;
//      state = ADDMIN;
//    }
//  }
//
//  else if(distance < minDis){
//    if(state == ADDMIN || state == INITIAL){
//      cycles++;
//      state = ADDMAX;
//    }
//  }
//  Serial.println(cycles);
//  if (cycles > 20) {
//    // reach 20, reset to 0, got fish successfully or failed
//    cycles = 0;
//    Serial.println("1");
//    delay(500);
//  }

  while(Serial.available())
  {
    digitalWrite(EN_PIN_1, HIGH);
    char c = Serial.read();
    inputString += c;
    if (c == '\n') {
      stringComplete = true;   
    }

    if (stringComplete) {
      for (int i = 0; i < inputString.length(); i++) {
        if (inputString.substring(i, i + 1) == " ")
        {
          movement = inputString.substring(0, i).toInt();
          motor_speed = inputString.substring(i + 1).toInt();
          break;
        }
      }
      if (movement == 1) {
        usMotor_Status = BRAKE;
        motorGo(MOTOR_1, usMotor_Status, 0);
      }
      else if (movement == 2) {
        usMotor_Status = CW;
        motorGo(MOTOR_1, usMotor_Status, motor_speed);
      }
      else if (movement == 3) {
        usMotor_Status = CCW;
        motorGo(MOTOR_1, usMotor_Status, motor_speed);
      }
      else {
        Serial.println("In valid movement input.");
      }    
    stringComplete = false;
    inputString = "";
    }
  }
}

void motorGo(uint8_t motor, uint8_t direct, uint8_t pwm) {
  if(motor == MOTOR_1)
  {
    if(direct == CW)
    {
      digitalWrite(MOTOR_A1_PIN, LOW); 
      digitalWrite(MOTOR_B1_PIN, HIGH);
    }
    else if(direct == CCW)
    {
      digitalWrite(MOTOR_A1_PIN, HIGH);
      digitalWrite(MOTOR_B1_PIN, LOW);      
    }
    else
    {
      digitalWrite(MOTOR_A1_PIN, LOW);
      digitalWrite(MOTOR_B1_PIN, LOW);            
    }
    
    analogWrite(PWM_MOTOR_1, pwm); 
  }
}

