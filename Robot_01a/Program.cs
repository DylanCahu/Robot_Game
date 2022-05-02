using System;
using System.Reflection.Emit;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography;
using EnigmAlgo_Lib;
namespace Robot_01a
{
    class Program
    {

        static void AvancerAuBout()//sous programme
        {
            while (EnigmAlgo.PeutAvancer()) EnigmAlgo.Avancer();//tant qu'il peut avancer il avance
        }

        static bool EstUnChiffret(char car)//sous programme
        {
            if ((car > '0') && (car < '9'))
                 
                 return true;

            else return false;
        }

        static bool EstUneAction(char car)//sous programme
        {
            if ((car == 'A') || (car == 'B') || (car == 'D') || (car == 'G'))

                return true;

            else return false;
        }
        static void ExecuterAction(char car)//sous programme
        {
            if (car == 'D') { Console.WriteLine("A Droite!"); EnigmAlgo.TournerADroite(); }
            else if (car == 'G') { Console.WriteLine("A Gauche!"); EnigmAlgo.TournerAGauche(); }
            else if (car == 'A') { Console.WriteLine("On avance!"); EnigmAlgo.Avancer(); }
            else if (car == 'B') { Console.WriteLine("Tout droit!"); AvancerAuBout(); }
            else Console.WriteLine("commande inconnue: " + car);

            Console.WriteLine();
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            Console.WriteLine(" Détection de Robot rock (pas des daft punk) ");

            EnigmAlgo.Ouvrir();
            EnigmAlgo.ChargerExo(1);
            Console.WriteLine("[Entré] pour cheater");     Console.ReadLine();



            /* EnigmAlgo.TournerAGauche();
             for (compteur = 0; compteur < 13 ; compteur++) EnigmAlgo.Avancer();



             EnigmAlgo.ChargerExo(2);

             EnigmAlgo.TournerAGauche();for (compteur = 0; compteur < 7; compteur++) EnigmAlgo.Avancer();                            
             EnigmAlgo.TournerAGauche();for (compteur = 0; compteur < 6; compteur++) EnigmAlgo.Avancer();





             EnigmAlgo.ChargerExo(3);

             EnigmAlgo.TournerADroite();for (compteur = 0; compteur < 3; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerADroite();for (compteur = 0; compteur < 2; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerADroite();for (compteur = 0; compteur < 3; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerAGauche();for (compteur = 0; compteur < 2; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerAGauche();for (compteur = 0; compteur < 5; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerADroite();for (compteur = 0; compteur < 2; compteur++) EnigmAlgo.Avancer();


             EnigmAlgo.ChargerExo(4);

             EnigmAlgo.TournerADroite(); EnigmAlgo.Avancer();
             EnigmAlgo.TournerADroite(); for (compteur = 0; compteur < 9; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerADroite(); for (compteur = 0; compteur < 5; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerADroite(); for (compteur = 0; compteur < 2; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerAGauche(); for (compteur = 0; compteur < 2; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerAGauche(); for (compteur = 0; compteur < 2; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerADroite(); for (compteur = 0; compteur < 2; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerADroite(); for (compteur = 0; compteur < 4; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerAGauche(); for (compteur = 0; compteur < 2; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerAGauche(); for (compteur = 0; compteur < 4; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerADroite(); for (compteur = 0; compteur < 6; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerADroite(); for (compteur = 0; compteur < 6; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerADroite(); for (compteur = 0; compteur < 4; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerAGauche(); for (compteur = 0; compteur < 4; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerAGauche(); for (compteur = 0; compteur < 3; compteur++) EnigmAlgo.Avancer();
             EnigmAlgo.TournerAGauche(); for (compteur = 0; compteur < 1; compteur++) EnigmAlgo.Avancer();


             EnigmAlgo.ChargerExo(5);

             comp3 = 0;

             while (comp3 != 3)//fait 3 tours pour aller
             {
                 for (compteur = 0; compteur < 2; compteur++) EnigmAlgo.Avancer();
                 EnigmAlgo.TournerADroite(); for (compteur = 0; compteur < 5; compteur++) EnigmAlgo.Avancer();
                 EnigmAlgo.TournerADroite(); for (compteur = 0; compteur < 4; compteur++) EnigmAlgo.Avancer();
                 EnigmAlgo.TournerADroite(); for (compteur = 0; compteur < 5; compteur++) EnigmAlgo.Avancer();
                 EnigmAlgo.TournerADroite(); for (compteur = 0; compteur < 2; compteur++) EnigmAlgo.Avancer();
                 comp3++;
             }
             EnigmAlgo.TournerADroite(); for (compteur = 0; compteur < 3; compteur++) EnigmAlgo.Avancer();//va a la fin une fois les 3 tours fait 

             EnigmAlgo.ChargerExo(6);

             j = 2;

             EnigmAlgo.TournerAGauche(); EnigmAlgo.Avancer();

             while (j!=11) {
                 EnigmAlgo.TournerADroite(); for (compteur = 0; compteur < j; compteur++) EnigmAlgo.Avancer();
                 j++;
             } 

             EnigmAlgo.ChargerExo(7);

             EnigmAlgo.TournerADroite();
             EnigmAlgo.TournerADroite();

             h = 0;

             while (h != 10)
             {
                 h = h + 2;
                 EnigmAlgo.TournerADroite(); for (compteur = 0; compteur < h; compteur++) EnigmAlgo.Avancer();
                 EnigmAlgo.TournerADroite(); for (compteur = 0; compteur < h; compteur++) EnigmAlgo.Avancer();

             }

            EnigmAlgo.ChargerExo(8); //technique 1

            Choisir1eredirection();//voir ligne 16

            while (!EnigmAlgo.MissionEstTerminee()) //boucle infinie
            {
                AvancerAuBout(); //voir Ligne 9 

                choisirdirectionsuivante(); // voir ligne 19

            }
            */

            /*
              EnigmAlgo.ChargerExo(3);
               version 1
              string chemin ;
              //---prog qui suit le chemin décrit


             Console.WriteLine("entrez le chemin ");
             chemin = Console.ReadLine();


           for (int i = 0; i < chemin.Length; i++) //initialise le compteur
           {
               char lettre = chemin[i]; //raccoucie, lettre remplacera "+ Chemin[i] +" pour allez plus vite
               if      (lettre == 'D') { EnigmAlgo.TournerADroite(); Console.WriteLine("A droite!"); }
               else if (lettre == 'G') { Console.WriteLine("A Gauche!"); EnigmAlgo.TournerAGauche(); }
               else if (lettre == 'A') { Console.WriteLine("On avance!"); EnigmAlgo.Avancer();  }
               else if (lettre == 'B') { Console.WriteLine("Tout droit!"); AvancerAuBout();  }
               else Console.WriteLine("commande inconnue: " + lettre);
           }


              //version 2
              string chemin;
              Console.WriteLine("entrez le chemin ");
              chemin = Console.ReadLine();
              int N;

              for (int i = 0; i < chemin.Length; i++) //initialise le compteur
              {
                  char car = chemin[i]; //raccoucie, lettre remplacera "+ Chemin[i] +" pour allez plus vite

                  if ((car > '0') && (car < '9')); N= car-48;
                  Console.WriteLine("Chiffre "+ N);
                  if ((car >= 'A') && (car <= 'Z')) Console.WriteLine("Lettre = " + car);

                  if      (car == 'D') { Console.WriteLine("A Droite!"); EnigmAlgo.TournerADroite(); }
                  else if (car == 'G') { Console.WriteLine("A Gauche!"); EnigmAlgo.TournerAGauche(); }
                  else if (car == 'A') { Console.WriteLine("On avance!"); EnigmAlgo.Avancer(); }
                  else if (car == 'B') { Console.WriteLine("Tout droit!"); AvancerAuBout(); }
                  else Console.WriteLine("commande inconnue: " + car);
                  Console.WriteLine();
                  Console.WriteLine();
              }
              

            //version 3
            EnigmAlgo.ChargerExo(3);
            string chemin;
            Console.WriteLine("entrez le chemin ");
            chemin = Console.ReadLine();
            int N=1;//initialise a 1 de base pour ne pas avoir a mettre "1d3a..."

            for (int i = 0; i < chemin.Length; i++) //initialise le compteur
            {

                char car = chemin[i]; //raccoucie, car remplacera "+ Chemin[i] +" pour allez plus vite

                if ((car > '0') && (car < '9')) { N = car - 48;
                Console.WriteLine("Chiffre " + N); }
                else if ((car >= 'A') && (car <= 'Z'))
                { 
                    for (int j = 1; j <= N ; j++)
                    {

                        if      (car == 'D') { Console.WriteLine("A Droite!"); EnigmAlgo.TournerADroite(); }
                        else if (car == 'G') { Console.WriteLine("A Gauche!"); EnigmAlgo.TournerAGauche(); }
                        else if (car == 'A') { Console.WriteLine("On avance!"); EnigmAlgo.Avancer(); }
                        else if (car == 'B') { Console.WriteLine("Tout droit!"); AvancerAuBout(); }
                        else    Console.WriteLine("commande inconnue: " + car);
                       
                        Console.WriteLine();
                        Console.WriteLine();


                    }
                    N = 1;//remet a 1 pour la prochaine boucle
                }
               
            }
            */

            //version 4
            EnigmAlgo.ChargerExo(3);
            //string chemin="D3ADA";
            string chemin;
            Console.WriteLine("entrez le chemin ");
            chemin = Console.ReadLine();


            for (int i = 0; i < chemin.Length; i++) //initialise le compteur
            {

                char car1 = chemin[i]; //raccoucie, car remplacera "+ Chemin[i] +" pour allez plus vite
                Console.WriteLine("1> "+ car1 +"'" );

                if (EstUneAction (car1))
                {
                    ExecuterAction(car1) ;
                }
                else if ( EstUnChiffret (car1)) 
                {
                    int N = car1 - 48;
                    char car2 = chemin[i+1];
                    Console.WriteLine("2>> " + car2 + "'");

                    //execuer N fois l'action de la case suivante
                    //avancer de 2 cases
                    for (int j = 1; j <= N; j++)
                    {
                        ExecuterAction(car2);
                    }
                    i++;
                }
            }

            Console.WriteLine();  Console.Write("Fin Normale "); Console.ReadLine();
            EnigmAlgo.Fermer();

        }
    }

}