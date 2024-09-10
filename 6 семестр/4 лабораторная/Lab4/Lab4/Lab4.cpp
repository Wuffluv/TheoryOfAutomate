#include"Lib/FormalLanguage.h"
#include "Lib/Rule.h"


#include <iostream>
#include <vector>
#include <algorithm>

using namespace std;


int main()
{


    setlocale(LC_ALL, "rus");

    cout << "Лабораторная 4. Задание 1." << endl;
    list<Rule> dict =
    {
        Rule("V", "^M"),
        Rule("M", "^F"),
        Rule("M", "0C"),
        Rule("M", "1X"),
        Rule("C", "0N"),
        Rule("N", "1M"),
        Rule("B", "!N"),
        Rule("B", "&C"),
        Rule("X", "0S"),
        Rule("S", "1M"),
        Rule("Z", "vX"),
        Rule("Z", "^S"),

        //Rule("M","0S"),
    };

    //list<Rule> dict1 =
    //{
    //    Rule("A", "cC"),
    //    Rule("B", "dC"),
    //    Rule("D", "dC"),
    //    Rule("C", "cF"),
    //    Rule("C", "aE"),
    //    
    //    Rule("E", "~B"),
    //    Rule("E", "|I"),
    //    
    //    Rule("G", "|I"),
    //    Rule("F", "|I"),
    //    Rule("F", "&C"),
    //    Rule("H", "bE"),
    //    Rule("H", "aI"),
    //    Rule("J", "aG"),
    //    Rule("G", "~D"),
    //    Rule("J", "bI"),
    //    Rule("C", "bG"),

    //    
    //   
    //    

    //   
    //};

 

    FormalLanguage fl(dict);
    //FormalLanguage fl1(dict1);
    cout << "Правила автомата" << endl;
    fl.PrintRules();
    //fl1.PrintRules();
  //  fl1.buildGraph();
;
    // Удаление недостижимых состояний
    //fl.RemoveUnreachableRules('V');
    fl.RemoveUnreachableRules('V');
    cout << "Правила  после удаления недостижимых состоянии" << endl;
    // Вывод правил языка
    //fl.PrintRules();
  fl.PrintRules();
   //fl1.buildGraph();
   // fl.RemoveEquivalentStates();

  fl.RemoveEquivalentStates();

 // fl1.RemoveEquivalentStates();
  //fl1.RemoveUnreachableRules('A');
    cout << "Правила после удаления эквивалентных состоянии" << endl;
    // Вывод правил языка
    //fl.PrintRules();
    fl.PrintRules();
    //fl.buildGraph();
    //// Удаление недостижимых состояний
    //fl.RemoveUnreachableRules('V');
    //cout << "Правила" << endl;
    //// Вывод правил языка
    //fl.PrintRules();
}
