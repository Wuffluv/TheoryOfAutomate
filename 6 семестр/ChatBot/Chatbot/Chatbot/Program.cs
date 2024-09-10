using System;
using System.Collections.Generic;
using System.Linq;

public class FSM
{
    private string initial;
    private string current;
    private Dictionary<string, Dictionary<string, Transition>> states = new Dictionary<string, Dictionary<string, Transition>>();

    public FSM(string initial)
    {
        this.initial = initial;
        this.current = initial;
    }

    public void SetInitialState(string initial)
    {
        this.initial = initial;
        this.current = initial;
    }

    public void SetCurrentState(string current)
    {
        this.current = current;
    }

    public string GetCurrentState()
    {
        return current;
    }

    public void Reset()
    {
        current = initial;
    }

    public void AddState(string state, string symbol, string next, Action action)
    {
        if (!states.ContainsKey(state))
        {
            states[state] = new Dictionary<string, Transition>();
        }

        states[state][symbol] = new Transition(next, action);
    }

    public void RemoveState(string state, string symbol = "")
    {
        if (string.IsNullOrEmpty(symbol))
        {
            states.Remove(state);
        }
        else if (states.ContainsKey(state))
        {
            states[state].Remove(symbol);
        }
    }

    public virtual string Normalize(string symbol)
    {
        symbol = symbol.ToLower();
        if (symbol == "login" || symbol == "exit" || symbol == "say" || symbol == "memorize")
        {
            return symbol;
        }
        else
        {
            return "*";
        }
    }

    public void Process(string rawSymbol)
    {
        if (string.IsNullOrEmpty(rawSymbol))
            return;

        var state = states.ContainsKey(current) ? states[current] : null;
        if (state == null)
        {
            Console.WriteLine("Current state " + current + " is not defined.");
            return;
        }

        foreach (char rawChar in rawSymbol)
        {
            string symbol = rawChar.ToString();

            string normalizedSymbol = Normalize(symbol);

            if (!state.ContainsKey(normalizedSymbol) && state.ContainsKey("*"))
            {
                Console.WriteLine("Unrecognized symbol " + symbol + ", using *");
                normalizedSymbol = "*";
            }

            if (state.TryGetValue(normalizedSymbol, out var transition))
            {
                transition.Action?.Invoke();
                current = transition.Next;
            }
            else
            {
                Console.WriteLine("No transition for symbol " + symbol + " in state " + current);
                return;
            }
        }
    }

    private class Transition
    {
        public string Next { get; }
        public Action Action { get; }

        public Transition(string next, Action action)
        {
            Next = next;
            Action = action;
        }
    }
}

public class ChatBot
{
    private readonly FSM fsm;
    private readonly Dictionary<string, List<string>> sessions = new Dictionary<string, List<string>>();
    private readonly List<string> memories = new List<string>();
    private string login;

    public ChatBot()
    {
        fsm = new FSM("INIT");

        fsm.AddState("INIT", "*", "INIT", DoIntroduce);
        fsm.AddState("INIT", "LOGIN", "SESSION", DoLogin);
        fsm.AddState("INIT", "EXIT", "INIT", DoQuit);
        fsm.AddState("SESSION", "*", "SESSION", null);
        fsm.AddState("SESSION", "EXIT", "INIT", null);
        fsm.AddState("SESSION", "SAY", "SESSION", DoSay);
        fsm.AddState("SESSION", "MEMORIZE", "STORE", null);
        fsm.AddState("STORE", "*", "STORE", DoRemember);
        fsm.AddState("STORE", "EXIT", "SESSION", null);
    }

    public void Process(string input)
    {
        fsm.Process(input);
    }

    private void DoIntroduce()
    {
        Console.WriteLine("Please introduce yourself first!");
    }

    private void DoLogin()
    {
        Console.Write("Enter your username: ");
        string username = Console.ReadLine();
        Console.WriteLine("Welcome, " + username);
        login = username;
        sessions[login] = new List<string>();
    }

    private void DoSay()
    {
        if (sessions.ContainsKey(login))
        {
            Console.Write("Enter your message: ");
            string message = Console.ReadLine();
            sessions[login].Add(message);
        }
    }

    private void DoRemember()
    {
        Console.Write("Enter something to remember: ");
        string memory = Console.ReadLine();
        memories.Add(memory);
    }

    private void DoQuit()
    {
        Console.WriteLine("Bye bye!");
        Environment.Exit(0);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        ChatBot bot = new ChatBot();

        string input;
        while ((input = Console.ReadLine()) != null)
        {
            bot.Process(input);
        }
    }
}
