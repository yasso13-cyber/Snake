#include <iostream>
#include <conio.h>
#include <windows.h>
#include <vector>

using namespace std;

// Game state variables
bool gameOver;
const int width = 20;
const int height = 20;
int x, y, fruitX, fruitY, score;

// Structures to handle snake body tracking
struct Position {
    int x, y;
};
vector<Position> snakeBody;

enum eDirection { STOP = 0, LEFT, RIGHT, UP, DOWN };
eDirection dir;

void Setup() {
    gameOver = false;
    dir = STOP;
    x = width / 2;
    y = height / 2;
    fruitX = rand() % width;
    fruitY = rand() % height;
    score = 0;
    snakeBody.clear();
}

void Draw() {
    // Move cursor to top-left instead of clearing screen to prevent flickering
    COORD coord = {0, 0};
    SetConsoleCursorPosition(GetStdHandle(STD_OUTPUT_HANDLE), coord);

    // Top wall
    for (int i = 0; i < width + 2; i++) cout << "#";
    cout << endl;

    // Game grid
    for (int i = 0; i < height; i++) {
        for (int j = 0; j < width; j++) {
            if (j == 0) cout << "#"; // Left wall

            if (i == y && j == x) {
                cout << "O"; // Snake head
            } else if (i == fruitY && j == fruitX) {
                cout << "F"; // Fruit
            } else {
                bool printTail = false;
                for (const auto& block : snakeBody) {
                    if (block.x == j && block.y == i) {
                        cout << "o"; // Snake body segment
                        printTail = true;
                        break;
                    }
                }
                if (!printTail) cout << " ";
            }

            if (j == width - 1) cout << "#"; // Right wall
        }
        cout << endl;
    }

    // Bottom wall
    for (int i = 0; i < width + 2; i++) cout << "#";
    cout << endl;

    // Display scoreboard
    cout << "Score: " << score << "     " << endl;
    cout << "Controls: W (Up), S (Down), A (Left), D (Right), X (Exit)" << endl;
}

void Input() {
    if (_kbhit()) {
        switch (_getch()) {
            case 'a': case 'A': if (dir != RIGHT) dir = LEFT; break;
            case 'd': case 'D': if (dir != LEFT) dir = RIGHT; break;
            case 'w': case 'W': if (dir != DOWN) dir = UP; break;
            case 's': case 'S': if (dir != UP) dir = DOWN; break;
            case 'x': case 'X': gameOver = true; break;
        }
    }
}

void Logic() {
    if (dir == STOP) return;

    // Save current head position
    Position prevHead = {x, y};

    // Move head
    switch (dir) {
        case LEFT:  x--; break;
        case RIGHT: x++; break;
        case UP:    y--; break;
        case DOWN:  y++; break;
        default: break;
    }

    // Wall collision logic (Ends game if hit)
    if (x >= width || x < 0 || y >= height || y < 0) {
        gameOver = true;
        return;
    }

    // Self collision logic
    for (const auto& block : snakeBody) {
        if (block.x == x && block.y == y) {
            gameOver = true;
            return;
        }
    }

    // Update body segments
    if (!snakeBody.empty()) {
        snakeBody.insert(snakeBody.begin(), prevHead);
        if (x == fruitX && y == fruitY) {
            score += 10;
            fruitX = rand() % width;
            fruitY = rand() % height;
        } else {
            snakeBody.pop_back();
        }
    } else {
        if (x == fruitX && y == fruitY) {
            score += 10;
            snakeBody.push_back(prevHead);
            fruitX = rand() % width;
            fruitY = rand() % height;
        }
    }
}

int main() {
    // Hide terminal cursor for a cleaner look
    HANDLE consoleHandle = GetStdHandle(STD_OUTPUT_HANDLE);
    CONSOLE_CURSOR_INFO info;
    info.dwSize = 100;
    info.bVisible = FALSE;
    SetConsoleCursorInfo(consoleHandle, &info);

    Setup();
    while (!gameOver) {
        Draw();
        Input();
        Logic();
        Sleep(100); // Control game speed (lower = faster)
    }

    // Reset cursor visibility on exit
    info.bVisible = TRUE;
    SetConsoleCursorInfo(consoleHandle, &info);

    cout << "\nGame Over! Final Score: " << score << endl;
    return 0;
}
