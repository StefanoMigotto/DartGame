using Spectre.Console;
using Spectre.Console.Json;
using System.Text.Json;

namespace FreccetteNuovo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*
             * Inizializzare un file di salvataggio
             * Menu con nuovo gara o continua (se continua carica il file salvato)
             * 
             * Avere una classe Arcere
             * Classe società
             * e la data
             * 
             * GIOCO:
             * 
             * 3 FRECCETTE
             * 
             * tabella risultati
             * 
             * PUNTEGGIO 1 2 3 / PARZIALE (SOMMA DEI 3 PUNTEGGI)/ TOTALE
             * 
             * 
             * 
             */

            string filename = "save.dat";

            bool exit = true;
            while (exit)
            {
                var choose = AnsiConsole.Prompt(new SelectionPrompt<string>().Title($"Welcome, Choose an option").PageSize(5).AddChoices(new[] { $"New Game", $"Load the last saved", $"[yellow]View the last saved[/]", $"[red]Quit[/]" }));

                if (choose == "Load the last saved")
                {
                    if (File.Exists(filename))
                    {
                        ContinueGame();
                    }
                    else
                    {
                        AnsiConsole.Markup($"[red]ERRORE: [/][yellow]Don't exists saved file[/]");
                        Console.ReadLine();
                        Console.Clear();
                    }
                }
                else if (choose == "New Game")
                {
                    File.Create(filename).Close();
                    StartGame(false);

                }else if(choose == "[yellow]View the last saved[/]")
                {
                    ViewSavedFile();
                }
                else if (choose == "[red]Quit[/]")
                {
                    exit = false;
                }


            }
        }

        public static void ContinueGame()
        {
            string filename = File.ReadAllText("save.dat");
            Saved datasaved = JsonSerializer.Deserialize<Saved>(filename);

            var rule = new Rule("[green]Last Saved Data[/]");
            AnsiConsole.Write(rule);
            Table table = new Table();

            table.AddColumn("*").Centered();
            table.AddColumn("Arrow 1").Centered();
            table.AddColumn("Arrow 2").Centered();
            table.AddColumn("Arrow 3").Centered();
            table.AddColumn("Prog.").Centered();
            table.AddColumn("Tot.").Centered();
            table.AddColumn("10").Centered();
            table.AddColumn("X").Centered();

            for (int i = 0; i < 11; i++)
            {
                table.AddRow(
                    $"{i.ToString()}",
                    datasaved.Archer.ArcherScore[i].Arrow1.ToString(),
                    datasaved.Archer.ArcherScore[i].Arrow2.ToString(),
                    datasaved.Archer.ArcherScore[i].Arrow3.ToString(),
                    datasaved.Archer.ArcherScore[i].PartialScore.ToString(),
                    datasaved.Archer.ArcherScore[i].CalculatePartial().ToString(),
                    (datasaved.Archer.ArcherScore[i].Ten).ToString(),
                    (datasaved.Archer.ArcherScore[i].X).ToString()
                );
            }
            AnsiConsole.Write(table);

            AnsiConsole.Markup("[yellow]Press any button for continue...[/]");
            Console.ReadLine();

            

        }

        /// <summary>
        /// Savedclass si riferisce alla classe che contiene i vari dati che
        /// verranno salvati.
        /// </summary>
        /// <param name="savedClass"></param>
        public static void SaveFile(object savedClass)
        {
            File.WriteAllText("save.dat", JsonSerializer.Serialize(savedClass));
        }

        /// <summary>
        /// MODE == TRUE: skip introduction question
        /// </summary>
        /// <param name="mode"></param>
        public static void StartGame(bool mode)
        {
            Archer archer = new Archer();
            archer.ArcherScore = new List<Score>();
            Society society = new Society();
            Saved save = new Saved();


            if (!mode)
            {
                var rule = new Rule("[red]Basic Information[/]");
                AnsiConsole.Write(rule);
                archer.NameOfArcher = AnsiConsole.Ask<string>("Insert your name:");
                save.Archer = archer;
                society.NameOfSociety = AnsiConsole.Ask<string>("Insert the your club:");
                archer.Society = society;
                SaveFile(save);
            }

            int counterRace = 1;
            string QuestionPoint;
            Console.Clear();
            for (int i = 0; i < 12; i++)
            {
                var rule = new Rule("[green]Insert Data[/]");
                AnsiConsole.Write(rule);
                Score score = new Score();
                score.Arrow1 = AnsiConsole.Prompt(new TextPrompt<int>($"[yellow]{counterRace} - Insert the 1 arrow:[/] ").Validate(score =>
                {
                    return score switch
                    {
                        > 10 => ValidationResult.Error("[red]10 Is the max value.[/]"),
                        _ => ValidationResult.Success()
                    };
                }
                ));
                if(score.Arrow1  == 10)
                {
                    CheckValueTen(score);
                }

                score.Arrow2 = AnsiConsole.Prompt(new TextPrompt<int>($"[yellow]{counterRace} - Insert the 2 arrow:[/] ").Validate(score =>
                {
                    return score switch
                    {
                        > 10 => ValidationResult.Error("[red]10 Is the max value.[/]"),
                        _ => ValidationResult.Success()
                    };
                }
));
                if (score.Arrow2 == 10)
                {
                    CheckValueTen(score);
                }

                score.Arrow3 = AnsiConsole.Prompt(new TextPrompt<int>($"[yellow]{counterRace} - Insert the 1 arrow:[/] ").Validate(score =>
                {
                    return score switch
                    {
                        > 10 => ValidationResult.Error("[red]10 Is the max value.[/]"),
                        _ => ValidationResult.Success()
                    };
                }
));
                if (score.Arrow3 == 10)
                {
                    CheckValueTen(score);
                }

                score.PartialScore = score.CalculatePartial();
                save.Score = score;



                counterRace++;
                archer.ArcherScore.Add(score);
                SaveFile(save);
                Console.Clear();
            }
            Console.Clear();

            PrintResultOfGame(save,archer);
        }

        public static void ViewSavedFile()
        {
            string filename = "save.dat";
            if(File.Exists(filename))
            {
                string SavedJson = File.ReadAllText(filename);
                var json = new JsonText(SavedJson);

                AnsiConsole.Write(
                    new Panel(json)
                        .Header("Saved File")
                        .Collapse()
                        .RoundedBorder()
                        .PadRight(10)
                        .BorderColor(Color.Yellow));

                string FileChoose = AnsiConsole.Prompt(new SelectionPrompt<string>().Title($"[yellow]How do you want continue?[/]").PageSize(5).AddChoices(new[] { $"[yellow]Back to the menu[/]", $"[red]Delete the saved file.[/]" }));
                if (FileChoose == "[red]Delete the saved file.[/]")
                {
                    File.Delete(filename);
                    AnsiConsole.Markup("[green]File deleted successfully! Press any key for continue...[/]");
                    Console.ReadLine();
                }
            }
            else
            {
                AnsiConsole.Markup("[red]The saved file not exist, press any key for continue...[/]");
                Console.ReadLine();
                
            }
        }

        /// <summary>
        /// Check if user type X or 10 and set the value correct
        /// </summary>
        /// <param name="score"></param>
        public static void CheckValueTen(Score score)
        {
            string QuestionPoint = AnsiConsole.Prompt(new SelectionPrompt<string>().Title($"[yellow]How do you want catalogue this data?[/]").PageSize(5).AddChoices(new[] { $"[yellow]X[/]", $"[yellow]10[/]" }));
            if (QuestionPoint == "[yellow]X[/]")
            {
                score.X = true;
                score.XTotal += 1;
            }

            if (QuestionPoint == "[yellow]10[/]")
            {
                score.Ten = true;
                score.TenTotal += 1;
            }
                
        }

        /// <summary>
        /// this function is for print the resutl scoreboard and
        /// calculate the total
        /// </summary>
        /// <param name="save"></param>
        /// <param name="archer"></param>
        public static void PrintResultOfGame(Saved save,Archer archer)
        {
            var rule = new Rule("[yellow]Result Game[/]");
            AnsiConsole.Write(rule);
            Table table = new Table();
            table.AddColumn("*").Centered();
            table.AddColumn("Arrow 1").Centered();
            table.AddColumn("Arrow 2").Centered();
            table.AddColumn("Arrow 3").Centered();
            table.AddColumn("Prog.").Centered();
            table.AddColumn("Tot.").Centered();
            table.AddColumn("10").Centered();
            table.AddColumn("X").Centered();

            List<int> ListOfTotalIntegers = new List<int>();

            for (int i = 0; i < 12; i++)
            {
                if (i == 0)
                {
                    table.AddRow($"[yellow]{i.ToString()}[/]",
                        archer.ArcherScore[i].Arrow1.ToString(),
                        archer.ArcherScore[i].Arrow2.ToString(),
                        archer.ArcherScore[i].Arrow3.ToString(),
                        (archer.ArcherScore[i].Arrow1 + archer.ArcherScore[i].Arrow2 + archer.ArcherScore[i].Arrow3).ToString(),
                        (archer.ArcherScore[i].Arrow1 + archer.ArcherScore[i].Arrow2 + archer.ArcherScore[i].Arrow3).ToString(),
                        (archer.ArcherScore[i].TenTotal).ToString(),
                        (archer.ArcherScore[i].XTotal).ToString()
                    );
                }
                else
                {

                    // I = 0 J = 1-1
                    // I = 1 J = 2-1
                    // I = 2 J = 3-1
                    // I = 3 j = 4-1

                    int TotalValue = archer.ArcherScore[i].Arrow1 + archer.ArcherScore[i].Arrow2 + archer.ArcherScore[i].Arrow3;

                    for (int j = 0; j <= 11; j++)
                    {
                        if (j != 0)
                        {
                            TotalValue += archer.ArcherScore[j - 1].Arrow1 + archer.ArcherScore[j - 1].Arrow2 + archer.ArcherScore[j - 1].Arrow3;
                        }

                        ListOfTotalIntegers.Add(TotalValue);

                    }
                    table.AddRow($"[yellow]{i.ToString()}[/]",
                        archer.ArcherScore[i].Arrow1.ToString(),
                        archer.ArcherScore[i].Arrow2.ToString(),
                        archer.ArcherScore[i].Arrow3.ToString(),
                        (archer.ArcherScore[i].Arrow1 + archer.ArcherScore[i].Arrow2 + archer.ArcherScore[i].Arrow3).ToString(),
                        ListOfTotalIntegers[i].ToString(),
                        (archer.ArcherScore[i].TenTotal).ToString(),
                        (archer.ArcherScore[i].XTotal).ToString()
                    );
                }
            }
            AnsiConsole.Write(table);
            SaveFile(save);
        }

    }
}
