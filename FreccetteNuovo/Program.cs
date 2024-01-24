using Spectre.Console;
using System.Text.Json;
using Spectre.Console.Json;

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
                var choose = AnsiConsole.Prompt(new SelectionPrompt<string>().Title($"Benvenuto, Come desideri procede?").PageSize(5).AddChoices(new[] { $"Nuova Partita", $"Carica il vecchio salvataggio", $"[yellow]Visualizza file di salvataggio[/]", $"[red]Esci[/]" }));

                if (choose == "Carica il vecchio salvataggio")
                {
                    if (File.Exists(filename))
                    {
                        ContinueGame();
                    }
                    else
                    {
                        AnsiConsole.Markup($"[red]ERRORE: [/][yellow]Non ci sono file di salvataggio trovati.[/]");
                        Console.ReadLine();
                        Console.Clear();
                    }
                }
                else if (choose == "Nuova Partita")
                {
                    File.Create(filename).Close();
                    StartGame(false);

                }else if(choose == "[yellow]Visualizza file di salvataggio[/]")
                {
                    ViewSavedFile();
                }
                else if (choose == "[red]Esci[/]")
                {
                    exit = false;
                }


            }
        }

        public static void ContinueGame()
        {
            string filename = File.ReadAllText("save.dat");
            var datasaved = JsonSerializer.Deserialize<Saved>(filename);
            StartGame(true);
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
                archer.NameOfArcher = AnsiConsole.Ask<string>("Inserisci il tuo nome:");
                save.Archer = archer;
                society.NameOfSociety = AnsiConsole.Ask<string>("Inserisci il nome della tua società:");
                archer.Society = society;
                SaveFile(save);
            }

            int counterRace = 1;
            string QuestionPoint;
            Console.Clear();
            for (int i = 0; i < 12; i++)
            {
                Score score = new Score();
                score.Arrow1 = AnsiConsole.Ask<int>($"[yellow]{counterRace} - Inserisci il punteggio della 1 freccia:[/] ");
                if(score.Arrow1 > 9)
                {
                    CheckValueTen(score);
                }
                    
                score.Arrow2 = AnsiConsole.Ask<int>($"[yellow]{counterRace} - Inserisci il punteggio della 2 freccia:[/] ");
                if (score.Arrow2 > 9)
                {
                    CheckValueTen(score);
                }

                score.Arrow3 = AnsiConsole.Ask<int>($"[yellow]{counterRace} - Inserisci il punteggio della 3 freccia:[/] ");
                if (score.Arrow3 > 9)
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

                string FileChoose = AnsiConsole.Prompt(new SelectionPrompt<string>().Title($"[yellow]Come vuoi continuare?[/]").PageSize(5).AddChoices(new[] { $"[yellow]Cancella il salvataggio[/]", $"[yellow]Continua e torna al menu[/]" }));
                if (FileChoose == "[yellow]Cancella il salvataggio[/]")
                {
                    File.Delete(filename);
                }
            }
            else
            {
                AnsiConsole.Markup("[red]Non ci sono file di configurazione.[/]");
                Console.ReadLine();
                
            }
        }

        /// <summary>
        /// Check if user type X or 10 and set the value correct
        /// </summary>
        /// <param name="score"></param>
        public static void CheckValueTen(Score score)
        {
            string QuestionPoint = AnsiConsole.Prompt(new SelectionPrompt<string>().Title($"[yellow]Come vuoi catalogare questo dato?[/]").PageSize(5).AddChoices(new[] { $"[yellow]X[/]", $"[yellow]10[/]" }));
            if (QuestionPoint == "[yellow]X[/]")
                score.X = true;
            if (QuestionPoint == "[yellow]10[/]")
                score.Ten = true;
        }

        /// <summary>
        /// this function is for print the resutl scoreboard and
        /// calculate the total
        /// </summary>
        /// <param name="save"></param>
        /// <param name="archer"></param>
        public static void PrintResultOfGame(Saved save,Archer archer)
        {
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
                        (archer.ArcherScore[i].Ten ? 'X' : '-').ToString(),
                        (archer.ArcherScore[i].X ? 'X' : '-').ToString()
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
                        (archer.ArcherScore[i].Ten ? 'X' : '-').ToString(),
                        (archer.ArcherScore[i].X ? 'X' : '-').ToString()
                    );
                }
            }
            AnsiConsole.Write(table);
            SaveFile(save);
        }

    }
}
