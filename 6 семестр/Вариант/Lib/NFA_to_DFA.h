#include <iostream>
#include <vector>
#include <string>
#include <set>
using namespace std;

// Входные переменные
vector<vector<string>> transition_table; // Таблица переходов НКА
vector<string> states; // Состояния НКА
vector<string> alphabets; // Алфавит НКА
string start_state; // Начальное состояние НКА

// Выходные переменные
vector<pair<string, vector<string>>> dfa_table; // Таблица переходов ДКА
vector<pair<string, bool>> completed; // Флаги завершения состояний ДКА

// Прототипы функций
void take_input();
void print_output();
void create_state_transitions(string state);
string unite(vector<string> store_state);
string fetch(char c, int alphabet);
bool isCompleted(string state);
void print_dfa();

//int main() {
//    setlocale(LC_ALL, "RU");
//    take_input();
//    print_output();
//    create_state_transitions(start_state);
//    print_dfa();
//    return 0;
//}

//считывание входных данных 
void take_input() {
    int number_of_states, number_of_alphabets;
    string state;

    cout << "Количество состояний: ";
    cin >> number_of_states;
    cout << "Введите состояния: ";
    for (int i = 0; i < number_of_states; i++) {
        cin >> state;
        states.push_back(state);
    }

    cout << "Количество символов алфавита: ";
    cin >> number_of_alphabets;
    cout << "Введите символы алфавита: ";
    string alphabet;
    for (int i = 0; i < number_of_alphabets; i++) {
        cin >> alphabet;
        alphabets.push_back(alphabet);
    }

    cout << "Введите таблицу переходов НКА (БЕЗ ЗАПЯТЫХ!):\n";
    for (int i = 0; i < states.size(); i++) {
        vector<string> row_states;
        for (int j = 0; j < alphabets.size(); j++) {
            cin >> state;
            row_states.push_back(state);
        }
        transition_table.push_back(row_states);
    }

    cout << "Введите начальное состояние: ";
    cin >> start_state;
}

void take_input_static() {
    int number_of_states = 4, number_of_alphabets = 2;
    string state;
    cout << "Количество состояний: 4 " << endl;
    states.push_back("S");
    states.push_back("A");
    states.push_back("B");
    states.push_back("H");

    cout << "Количество символов алфавита: 2 "<< endl;
    string alphabet;
    alphabets.push_back("0");
    alphabets.push_back("1");


    vector<string> row_statesS;
    row_statesS.push_back("NULL");
    row_statesS.push_back("NULL");
    transition_table.push_back(row_statesS);
    
    vector<string> row_statesA;
    row_statesA.push_back("SA");
    row_statesA.push_back("SS");
    transition_table.push_back(row_statesA);

    vector<string> row_statesB;
    row_statesB.push_back("AA");
    row_statesB.push_back("AB");
    transition_table.push_back(row_statesB);

    vector<string> row_statesH;
    row_statesH.push_back("S");
    row_statesH.push_back("SA");
    transition_table.push_back(row_statesH);

               
    start_state = "H";
}
//вывод входных данных
void print_output() {
    cout << "Состояния: ";
    for (int i = 0; i < states.size(); i++) cout << states[i] << " ";
    cout << endl;
    cout << "Алфавит: ";
    for (int i = 0; i < alphabets.size(); i++) cout << alphabets[i] << " ";
    cout << endl;

    cout << "Таблица переходов:\n";
    cout << "\t   0 \t 1\n";
    for (int i = 0; i < states.size(); i++) {
        cout << states[i] << "\t: ";
        for (int j = 0; j < alphabets.size(); j++) {
            cout << transition_table[i][j] << "\t";
        }
        cout << "\n";
    }
    cout << "Начальное состояние: " << start_state;
    cout << endl;
}
//проверка, было ли состояние уже обработано и добавлено в таблицу переходов ДКА
bool isCompleted(string state) {
    for (int i = 0; i < completed.size(); i++) {
        if (completed[i].first == state) return completed[i].second;
    }
    return false; // Состояние еще не создано.
}
// принимает символ алфавита и номер алфавита и возвращает состояние, 
// в которое происходит переход из данного состояния по данному символу
string fetch(char c, int alphabet) {
    string s(1, c);
    int state_index = 0;
    for (int i = 0; i < states.size(); i++)
        if (states[i] == s) {
            state_index = i;
            break;
        }

    return transition_table[state_index][alphabet];
}
//принимает вектор состояний и объединяет их в одно состояние. 
// Она удаляет дублирующиеся символы и возвращает новое состояние.
string unite(vector<string> store_state) {
    // Разбиение всех объединенных состояний на символы
    set<char> split_s;
    for (int i = 0; i < store_state.size(); i++) {
        if (store_state[i] == "NULL") continue;
        for (int j = 0; j < store_state[i].size(); j++) {
            split_s.insert(store_state[i][j]);
        }
    }

    // Формирование нового состояния
    string new_state = "";
    for (auto element : split_s) new_state += element;
    return new_state;
}

//рекурсивно создает таблицу переходов ДКА. 
// Она принимает состояние в качестве аргумента и проверяет,
//  было ли оно уже обработано.
//  Если нет, то создает новую строку в таблице переходов ДКА.
void create_state_transitions(string state) {
    if (isCompleted(state))return;

    vector<string> dfa_row;
    for (int a = 0; a < alphabets.size(); a++) {
        vector<string>store_state;
        // Для каждого символа алфавита функция вызывает fetch() 
        // для каждого состояния и сохраняет результат в вектор store_state.
        for (int w = 0; w < state.size(); w++) {
            store_state.push_back(fetch(state[w], a));
        }
        //вызывается функция unite() для вектора store_state, чтобы получить новое объединенное состояние.
        string new_state = unite(store_state);

        dfa_row.push_back(new_state);
    }
    //новая строка таблицы переходов ДКА добавляется в вектор dfa_table
    dfa_table.push_back(make_pair(state, dfa_row));
    //Состояние помечается как завершенное, чтобы избежать повторной обработки.
    completed.push_back(make_pair(state, true));

    //рекурсивно вызывается create_state_transitions() для каждого нового состояния в строке таблицы переходов ДКА, если оно не является "NULL"
    for (int i = 0; i < dfa_row.size(); i++)
        if (dfa_row[i] != "NULL")
            create_state_transitions(dfa_row[i]);
}

//вывод таблицы переходов ДКА
void print_dfa() {
    cout << "Таблица переходов ДКА :\n";
    cout << "\t 0 \t 1\n";
    for (int i = 0; i < dfa_table.size(); i++) {
        cout << dfa_table[i].first << "\t:";
        for (int j = 0; j < dfa_table[i].second.size(); j++) {
            cout << dfa_table[i].second[j] << "\t";
        }
        cout << endl;
    }
    cout << endl;
}