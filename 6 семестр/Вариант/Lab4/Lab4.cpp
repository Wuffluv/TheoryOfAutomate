#include "../Lib/Rule.h"
#include "../Lib/Minimization.h"

#include <iostream>

int main()
{
    setlocale(LC_ALL, "RUS");

    // Знак перпендикулляра = +
    list<Rule> dict1 =
    {
         Rule("S", "_A"),
         Rule("A", "96N"),
         Rule("A", "0D"),
         Rule("D", "1F"),
         Rule("D", "aD"),
         Rule("D", "bD"),
         Rule("F", "0D"),
         Rule("F", "96N"),
         Rule("B", "1A"),
         Rule("B", "0A"),
         Rule("B", "aB"),
         Rule("B", "bB"),
         Rule("A", "1B"),
         Rule("A", "96N"),
         Rule("E", "1A"),
         Rule("E", "0D"),
         
    };

    PrintRules(dict1);

    // Создаем формальный язык
    FormalLanguage language(dict1);

    // Выводим формальный язык до устранения недостижимых состояний
    std::cout << "Before elimination:" << std::endl;
    std::cout << language.OutputLeft() << std::endl;

    // Устраняем недостижимые состояния
    language.RemoveUnreachableStates();

    // Выводим формальный язык после устранения недостижимых состояний
    std::cout << "After elimination:" << std::endl;
    std::cout << language.OutputLeft() << std::endl;

    return 0;
}