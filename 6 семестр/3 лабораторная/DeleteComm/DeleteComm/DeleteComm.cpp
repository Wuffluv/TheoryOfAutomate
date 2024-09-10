#include <iostream>
#include <fstream>
#include <string>

using namespace std;

enum class State {
    Normal_mode,
    One_liner,
    Char_literal,
    String_literal
};

void processFile(const string& inputFileName, const string& outputFileName) {
    ifstream inputFile(inputFileName);
    ofstream outputFile(outputFileName);

    if (!inputFile) {
        cerr << "Ошибка открытия файла" << endl;
        return;
    }

    if (!outputFile) {
        cerr << "Ошибка открытия файла" << endl;
        return;
    }

    State state = State::Normal_mode;
    bool ignoreNextChar = false;

    char c;
    while (inputFile.get(c)) {
        switch (state) {
        case State::Normal_mode:
            if (c == '#') {
                state = State::One_liner;
            }
            else if (c == '\"') {
                state = State::String_literal;
            }
            else if (c == '\'') {
                state = State::Char_literal;
            }
            else {
                outputFile << c;
            }
            break;

        case State::One_liner:
            if (c == '\n') {
                state = State::Normal_mode;
                outputFile << c;
            }
            break;

        case State::Char_literal:
            if (ignoreNextChar) {
                ignoreNextChar = false;
            }
            else {
                if (c == '\'') {
                    state = State::Normal_mode;
                }
                else {
                    ignoreNextChar = (c == '\'');
                }
            }
            break;

        case State::String_literal:
            if (ignoreNextChar) {
                ignoreNextChar = false;
            }
            else {
                if (c == '\"') {
                    state = State::Normal_mode;
                }
                else {
                    ignoreNextChar = (c == '\"');
                }
            }
            break;
        }
    }
    inputFile.close();
    outputFile.close();
}

int main() {
    setlocale(LC_ALL, "Russian");
    string inputFileName = "input.txt";
    string outputFileName = "output2.txt";
    processFile(inputFileName, outputFileName);

    cout << "Комментарии удалены" << endl;

    return 0;
}