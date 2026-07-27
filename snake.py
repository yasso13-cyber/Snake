import turtle
import time
import random

# Game configuration
DELAY = 0.1
SCORE = 0
HIGH_SCORE = 0

# Set up the screen
screen = turtle.Screen()
screen.title("Snake Game")
screen.bgcolor("#f0ebe6") # Paper-colored background
screen.setup(width=600, height=600)
screen.tracer(0) # Turns off screen updates for smooth movement

# Snake Head
head = turtle.Turtle()
head.speed(0)
head.shape("square")
head.color("#282828") # Dark pencil grey
head.penup()
head.goto(0, 0)
head.direction = "stop"

# Snake Food
food = turtle.Turtle()
food.speed(0)
food.shape("circle")
food.color("#c83232") # Red felt marker color
food.penup()
food.goto(0, 100)

# Snake Body Segments
segments = []

# Score Pen
pen = turtle.Turtle()
pen.speed(0)
pen.shape("square")
pen.color("#282828")
pen.penup()
pen.hideturtle()
pen.goto(0, 260)
pen.write("Score: 0  High Score: 0", align="center", font=("Arial", 24, "bold"))

# Movement Functions
def go_up():
    if head.direction != "down":
        head.direction = "up"

def go_down():
    if head.direction != "up":
        head.direction = "down"

def go_left():
    if head.direction != "right":
        head.direction = "left"

def go_right():
    if head.direction != "left":
        head.direction = "right"

def move():
    if head.direction == "up":
        y = head.ycor()
        head.sety(y + 20)

    if head.direction == "down":
        y = head.ycor()
        head.sety(y - 20)

    if head.direction == "left":
        x = head.xcor()
        head.setx(x - 20)

    if head.direction == "right":
        x = head.xcor()
        head.setx(x + 20)

# Keyboard bindings
screen.listen()
screen.onkeypress(go_up, "Up")
screen.onkeypress(go_down, "Down")
screen.onkeypress(go_left, "Left")
screen.onkeypress(go_right, "Right")

# Main Game Loop
while True:
    screen.update()

    # Check for wall collisions
    if head.xcor() > 290 or head.xcor() < -290 or head.ycor() > 290 or head.ycor() < -290:
        time.sleep(1)
        head.goto(0, 0)
        head.direction = "stop"

        # Hide the old body segments off-screen
        for segment in segments:
            segment.goto(1000, 1000)
        segments.clear()

        # Reset score
        SCORE = 0
        pen.clear()
        pen.write(f"Score: {SCORE}  High Score: {HIGH_SCORE}", align="center", font=("Arial", 24, "bold"))

    # Check for eating food
    if head.distance(food) < 20:
        # Move food to random position
        x = random.randint(-280, 280) // 20 * 20
        y = random.randint(-280, 280) // 20 * 20
        food.goto(x, y)

        # Add a new body segment
        new_segment = turtle.Turtle()
        new_segment.speed(0)
        new_segment.shape("square")
        new_segment.color("#404040") # Slightly lighter grey for body
        new_segment.penup()
        segments.append(new_segment)

        # Increase score
        SCORE += 1
        if SCORE > HIGH_SCORE:
            HIGH_SCORE = SCORE
        pen.clear()
        pen.write(f"Score: {SCORE}  High Score: {HIGH_SCORE}", align="center", font=("Arial", 24, "bold"))

    # Move the body segments in reverse order
    for index in range(len(segments) - 1, 0, -1):
        x = segments[index-1].xcor()
        y = segments[index-1].ycor()
        segments[index].goto(x, y)

    # Move segment 0 to where the head was
    if len(segments) > 0:
        x = head.xcor()
        y = head.ycor()
        segments[0].goto(x, y)

    move()

    # Check for body self-collisions
    for segment in segments:
        if segment.distance(head) < 20:
            time.sleep(1)
            head.goto(0, 0)
            head.direction = "stop"
            
            for seg in segments:
                seg.goto(1000, 1000)
            segments.clear()

            SCORE = 0
            pen.clear()
            pen.write(f"Score: {SCORE}  High Score: {HIGH_SCORE}", align="center", font=("Arial", 24, "bold"))

    time.sleep(DELAY)
