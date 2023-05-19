using System.Diagnostics;
using MathGame.Services;
using MathGame.UserInput;
using Spectre.Console;

namespace MathGame.Controllers;

public class GameController
{
    private readonly Input _input = new();
    private readonly Calculator _calculator = new();

    readonly List<string> _gameHistory = new();

    private Operation Menu()
    {
        Console.Clear();

        var option = AnsiConsole.Prompt(
            new SelectionPrompt<Operation>()
            .Title("Select your choice")
            .AddChoices(Operation.Addition,
                Operation.Subtraction,
                Operation.Multiplication,
                Operation.Division, 
                Operation.History,
                Operation.Exit));

        return option;
    }

    public void Play()
    {
        var choice = Menu();

        while (true)
        {
            switch (choice)
            {
                case Operation.Exit:
                    return;
                case Operation.Addition:
                    PlayGameRound(Operation.Addition);
                    break;
                case Operation.Subtraction:
                    PlayGameRound(Operation.Subtraction);
                    break;
                case Operation.Multiplication:
                    PlayGameRound(Operation.Multiplication);
                    break;
                case Operation.Division:
                    PlayGameRound(Operation.Division);
                    break;
                case Operation.History:
                    ViewHistory();
                    break;
                default:
                    DisplayInvalidChoiceError();
                    break;
            }

            choice = Menu();
        }
    }

    private void ViewHistory()
    {
        Console.Clear();
        foreach (var game in _gameHistory)
        {
            Console.WriteLine(game);
        }

        Console.WriteLine("Press Enter to continue");
        Console.ReadLine();
    }

    private static void DisplayInvalidChoiceError()
    {
        Console.WriteLine("Error! Invalid Choice!");
        Console.WriteLine("Press Enter to continue");
        Console.ReadLine();
    }

    private void PlayGameRound(Operation operation)
    {
        Console.WriteLine("How many questions do you want?");
        int count = GetNumberOfQuestions();
        
        var score = 0;

        var timer = new Stopwatch();
        timer.Start();

        score = GameLoop(operation, count, score);

        timer.Stop();

        int time = (int)timer.Elapsed.TotalSeconds;

        Console.WriteLine($"You scored {score} out of {count} in {time} seconds");
        _gameHistory.Add($"Game: {operation} | Score: {score} out of {count} | Duration: {time} seconds");
        Console.WriteLine("Press Enter to continue: ");
        Console.ReadLine();
    }

    private int GameLoop(Operation operation, int count, int score)
    {
        for (int i = 0; i < count; i++)
        {
            var numA = Random.Shared.Next(1, 10);
            var numB = Random.Shared.Next(1, 10);
            if (operation == Operation.Division)
            {
                numA *= numB;
            }

            var question = CreateQuestion(numA, numB, operation);

            var answer = _calculator.Calculate(numA, numB, operation);
            Console.WriteLine(question);
            var userAnswer = _input.GetInput();

            if (userAnswer == answer.ToString())
            {
                Console.WriteLine("Correct!");
                score++;
            }
            else
            {
                Console.WriteLine("Wrong!");
                Console.WriteLine($"The answer was {answer}");
            }
        }

        return score;
    }

    private int GetNumberOfQuestions() => _input.GetChoice();

    private string CreateQuestion(int numA, int numB, Operation operation)
    {
        switch (operation)
        {
            case Operation.Addition:
                return $"{numA} + {numB}";
            case Operation.Subtraction:
                return $"{numA} - {numB}";
            case Operation.Multiplication:
                return $"{numA} * {numB}";
            case Operation.Division:
                return $"{numA} / {numB}";
            default:
                return "No question";
        }
    }
}