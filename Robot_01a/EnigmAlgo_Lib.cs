////////////////////////////////////////////////////////////////////////////////
//
//  Unité de Communication avec l'Application EnigmAlgo
//  Raduction du Code Source Pascal-Delphi-5 en C#
//                                                         ID  2020/09/20-19:25
//------------------------------------------------------------------------------
//
//  Structure des messages envoyés par SendMessage
//    LRESULT SendMessage(
//      HWND   hWnd,	// handle of destination window // Handle de l'appli cible
//      UINT   Msg,	// message to send              // toujours WM_USER
//      WPARAM wParam,	// first message parameter      // WORD     = 2 octets
//      LPARAM lParam 	// second message parameter     // LONGWORD = 4 octetes
//    );
//
//   Type  TMessageOption = ( Possible  ,  // L'action est-elle possible ?
//                            Effectuer ,  // Effectuer l'action !
//                            Consulter    // Consultation d'un élément (ou d'un état)
//
//  Option permet de moduler l'action demandée (Possible, Effecter)
//  Consulter un état du jeu ou une donnée
//
//  Type  TMessageAction = ( _FirstAction   ,  // Ne rien faire (première action)
//                           _PasserEnModeExo, // Indique le Mode de fonctionnement
//                           _ChargerExo    ,  // Charger l'exo demandé
//                           ...
//                           _LastAction       // Ne rien faire (dernière action)
//                         ) ;
//  Toutes les actions demandables ...
//
//  |      WPARAM       |  LPARAM                  |
//  | 1 octet | 1 octet |  4 octets                |
//  | Option  |  Action |  paramètres-de-l-action  |
//
////////////////////////////////////////////////////////////////////////////////
using System;
using System.Text;
using System.Windows;
using System.IO;
//using System.Windows.Forms;

namespace EnigmAlgo_Lib
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using WORD = System.UInt16;
    using LONGWORD = System.UInt32;

    public delegate bool CallBack(int hwnd, int lParam);

    class EnigmAlgo
    {
        //==============================================================================
        //
        //  Déclaration Externes pour Accéder aux Fonctions de Win32
        //
        //==============================================================================
        [DllImport("user32")]
        public static extern int EnumWindows(CallBack x, int y);

        [DllImport("user32.dll")]
        public static extern void GetWindowText(IntPtr hWnd, StringBuilder lpString, Int32 nMaxCount);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        public static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern bool CreateProcess(
           string lpApplicationName,
           string lpCommandLine,
           ref SECURITY_ATTRIBUTES lpProcessAttributes,
           ref SECURITY_ATTRIBUTES lpThreadAttributes,
           bool bInheritHandles,
           uint dwCreationFlags,
           IntPtr lpEnvironment,
           string lpCurrentDirectory,
           [In] ref STARTUPINFO lpStartupInfo,
           out PROCESS_INFORMATION lpProcessInformation);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct STARTUPINFO
        {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        //        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        //        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, StringBuilder lParam);

        //        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        //        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        //        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        //        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        //        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        //        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, ref IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, int wParam, IntPtr lParam);

        private const UInt16 WM_CLOSE = 0x0010;
        private const UInt16 WM_USER = 0x0400;

        //==============================================================================
        //
        //  Messages pour la Communication  (Partie commune avec EnigmAlgo)
        //
        //  Toutes les communications entre la console et EnigAlgo passent par le
        //  message WM_USER qui est envoyé au programme EnigmAlgo.
        //  Les paramètres de ce message WM_USER permettent sélectionner les actions
        //  à effectuer.
        //
        //  WM_USER       wParamHi  wParamLo  lParamHi  lParamLo  (tous en WORD !)
        //  obligatoire   Action    Option    Param2    Param3
        //
        //==============================================================================
        enum TMessageAction
        {
            _FirstAction, // Ne rien faire (première action)
            _AucuneAction, // Aucune action en cours (ou dernière action terminée)
            _PasserEnModeExo, // Indique le Mode de fonctionnement
            _ChargerExo,  // Charger l'exo demandé
            _ActeurAllerNord,
            _ActeurAllerSud,
            _ActeurAllerEst,
            _ActeurAllerOuest,
            _ActeurTournerDroite,  // Faire pivoter l'acteur à droite
            _ActeurTournerGauche,  // Faire pivoter l'acteur à gauche
            _ActeurGetDirection,  // Consulter la direction actuelle de l'acteur
            _ActeurOrienter,  // Orienter l'acteur vers une direction
            _ActeurAvancer,  // Faire avancer l'acteur
            _ActeurReculer,  // Faire reculer l'acteur
            _ActeurPousser,  // Faire pousser une caisse
            _ActeurOuvrir,  // Faire ouvrir  une caisse
            _ActeurLire,  // Faire Lire le contenu d'une caisse
            //  _ValeurSol     ,  // Consulter une case niveau du sol
            //  _ValeurMur     ,  // Consulter une case niveau des murs
            //  _ValeurSpeciale,  // Consulter une case niveau spécial
            _EstCaisseVersNord,
            _EstCaisseVersSud,
            _EstCaisseVersEst,
            _EstCaisseVersOuest,
            _PousserCaisseVersNord,
            _PousserCaisseVersSud,
            _PousserCaisseVersEst,
            _PousserCaisseVersOuest,

            _MissionEstTerminee, // La mission est-elle terminée
            _ExoCharger,        // Charger un exercice
            _LastAction        // Ne rien faire (dernière action)
        };

        enum TMessageOption
        {
            _Possible,   // L'action est-elle possible ?
            _Effectuer,  // Effectuer l'action !
            _Consulter   // Consultation d'un élément (ou d'un état)
        };
        //==============================================================================
        //
        //  Constantes de l'Unité
        //
        //==============================================================================
        //Const  EnigmAlgo_Dir         = 'C:\Temp\'  ;
        const string EnigmAlgo_Dir = "..\\..\\..\\..\\";  // "C:\\";
        //const string EnigmAlgo_Dir = "C:\\Users\\Patrick\\Documents\\_dev\\05_Enigma_Robot_Explosion\\";
        //Const  EnigmAlgo_AppName     =  EnigmAlgo_Dir + 'EnigmAlgo.exe' ;
        const string EnigmAlgo_WindowTitle = "EnigmAlgo";
        //==============================================================================
        //
        //  Variables Internes de l'Unité
        //
        //==============================================================================
        static private bool Internal_Error = false ;  // Indicateur d'erreur
        static private int Internal_AppHandle = 0 ;  // THandle de l'Application cible
        static private IntPtr Internal_MessageResult = IntPtr.Zero;  // LongInt : Retour du dernier message
        //==============================================================================
        //
        //  Ouverture et Fermeture de l'Application
        //
        //==============================================================================
        static public bool Erreur() //--------------------------------------------------
        //
        //  Renvoyer Vrai en cas d'erreur déjà rencontrée
        //------------------------------------------------------------------------------
        {
            return Internal_Error;
        }//-----------------------------------------------------------------------------
        static public void Attendre(int Milisec) //-------------------------------------
        //
        //  Attendre que la <Duree> (en milisecondes) soit écoulée
        //------------------------------------------------------------------------------
        {
            int StartTime = Environment.TickCount;  //: LongWord ;

            do
            {
                //      Application.ProcessMessages() ;
//                Application.DoEvents();
            }
            while (!((Environment.TickCount - StartTime) >= Milisec));
        }//--------------------------------------------------------------------------
        public static string GetText(IntPtr hwnd)
        {
            int length = GetWindowTextLength(hwnd);
            StringBuilder sb = new StringBuilder(length + 1);
            GetWindowText(hwnd, sb, sb.Capacity);
            return sb.ToString();
        }

        static private char   Callback_Option = '?';  // 'K'ill , 'F'ind
        static private string Callback_Title  = "" ;  // Titre de la fenêtre recherchée
        static private int    Callback_Handle =  0 ;  // THandle de l'application trouvée
        static private bool   Callback_Found  = false ;  // Resultat booléen de la recherche
        static private int    Callback_Cpt    =  0 ;  // debug
        static public bool Callback_EnumWindows(int hwnd, int lParam) //----------------
        //
        //  Fonction de rappel de l'énumération des fenetres
        //  [Callback_Title]  indique le titre de la fenêtre recherchée
        //  [Callback_Option] indique l'action à effectuer quand on a trouvé l'application
        //  [Callback_Handle] contiendra le Handle de la fenêtre trouvée (si trouvée)
        //  [Callback_Found]  contiendra Vrai (si la fenetre a été trouvée)
        //------------------------------------------------------------------------------
        {
            bool Result = true;
            if (Internal_Error) return Result;  // Sécurité !

            string TitreCourant = GetText((IntPtr)hwnd); // : ShortString;
//            Console.WriteLine("TitreCourant = " + hwnd + " " + TitreCourant);

//            if (TitreCourant != Callback_Title) return Result; // ce n'est pas la bonne application
//            if (String.Compare(TitreCourant,Callback_Title)!=0) return Result; // ce n'est pas la bonne application
            if (!TitreCourant.Equals(Callback_Title)) return Result; // ce n'est pas la bonne application

            //--- Ici, on a trouvé la bonne application (le même titre de fenêtre)
//            Console.WriteLine(">>>>>>>>>>>>> Trouvé ! <<<<<<<<<<<<<");
            Callback_Cpt++; // debug
            Result = false;

            switch (Callback_Option)
            {
                case 'K':
                case 'k':  //--- Kill - Détruire l'application trouvée
                    {
                        SendMessage((IntPtr)hwnd, (int)WM_CLOSE, 0, IntPtr.Zero); // WM_QUIT
                        Callback_Found = true;
                        break;
                    }
                case 'F':
                case 'f':  //--- Find - Retrouver le Handle de l'application
                    {
                        Callback_Handle = hwnd;
                        Callback_Found  = true;
                        break;
                    }
            }
            return Result;
        }//--------------------------------------------------------------------------
        static private void RetrouverApplication(string AppTitle) //--------------------
        //
        //  Rechercher l'application <AppTitle> dans les applications en cours
        //  d'exécution sous Windows.
        //  Si l'application est déjà lancée :
        //   - Callback_OK=True
        //   - Callback_Handle est le Handle de l'application trouvée
        //------------------------------------------------------------------------------
        {
//            Console.WriteLine();
//            Console.WriteLine("Debug : RetrouverApplication(" + AppTitle + ")");
            if (Internal_Error) return;  // Sécurité !

            CallBack myCallBack = new CallBack(Callback_EnumWindows);

            //--- Fixer les paramètres pour EnumWindows
            Callback_Title  = AppTitle;   // Titre de la fenêtre recherchée
            Callback_Option = 'F';   // 'K'ill , 'F'ind
            Callback_Handle =  0 ;   // Handle de l'application trouvée
            Callback_Found  = false;   // Resultat booléen de la recherche
            Callback_Cpt    =  0 ;     // debug
            EnumWindows(myCallBack, 0);    // Enumérer ...
//            Console.WriteLine("Callback_Cpt = " + Callback_Cpt);
        }//--------------------------------------------------------------------------
        static private void DemarrerApplication(string AppPath) //----------------------
        //
        //  Lancer l'application <AppPath>
        //  On ne verifie pas que l'application soit déjà lancée !
        //------------------------------------------------------------------------------
        {
            //Console.WriteLine("DemarrerApplication(" + AppPath + ")");

            if (Internal_Error) return;  // Sécurité !


            //--- Notepad
            const uint NORMAL_PRIORITY_CLASS = 0x0020;

            //string Application = @"C:\EnigmAlgo.exe";
            string Application = AppPath;
            string lesParams = "DISTANT";
            PROCESS_INFORMATION ProcessInfo = new PROCESS_INFORMATION();
            STARTUPINFO StartInfo = new STARTUPINFO();
            SECURITY_ATTRIBUTES pSec = new SECURITY_ATTRIBUTES();
            SECURITY_ATTRIBUTES tSec = new SECURITY_ATTRIBUTES();
            pSec.nLength = Marshal.SizeOf(pSec);
            tSec.nLength = Marshal.SizeOf(tSec);

            // Lancer l'exécutable EnigmAlgo
//            bool retValue = CreateProcess(Application, lesParams,
//            ref pSec, ref tSec, false, NORMAL_PRIORITY_CLASS,
//            IntPtr.Zero, null, ref StartInfo, out ProcessInfo);

            bool retValue = CreateProcess(null,Application+" "+lesParams,
            ref pSec, ref tSec, false, NORMAL_PRIORITY_CLASS,
            IntPtr.Zero, null, ref StartInfo, out ProcessInfo);
//            Console.WriteLine("Debug : lesParams = " + lesParams);

            if (retValue)
            {
                //--- L'application est bien lancée,
                //--- ProcessInfo.hProcess contient le handle du process principal de l'application
                Internal_AppHandle = (int)ProcessInfo.hProcess;
//                Console.WriteLine("L'application est bien lancée ! " + Internal_AppHandle);
                //Internal_Error = false;
            }
        }//--------------------------------------------------------------------------
        static private int GetConsoleHwnd() //------------------------------------------
        // : HWND ;
        //  Fonction trouvée sur Internet = une merdouillesque bidouille
        //  pour enfin pouvoir retrouver le handle de la fenetre console !!!
        //  C'est juste Hallucinant que ce soit aussi difficile et qu'aucune fonction de base de marche !!!
        //------------------------------------------------------------------------------
        {
            /*
            string NewWindowTitle;
            string OldWindowTitle;

            SetLength(NewWindowTitle, 1024);

            // Récupère le titre courant.
            GetConsoleTitle(PChar(OldWindowTitle), 1024);

            NewWindowTitle:= IntToStr(GetTickCount) + IntToStr(GetCurrentProcessId);
            //Writeln(OldWindowTitle);
            //Writeln(NewWindowTitle);
            // Change le titre.
            SetConsoleTitle(PChar(NewWindowTitle));

            // Attend pour que le titre de la fenêtre soit changé.
            Sleep(40);

            // Cherche le nouveau titre.
            Result:= FindWindow(null, PChar(NewWindowTitle));

            // Restaure le titre original.
            // Le titre originale de la console n'est pas correctement trouvé ! 
            SetConsoleTitle(PChar('Console EnigmAlgo'));
            */
            return 0;   // bidon
        }//-----------------------------------------------------------------------------
        static public string RetrouverCheminFichier(string FileName) //-----------------
        //
        //  Retrouver le chemin du fichier <FileName> en remontant l'arborescence.
        //  Renvoyer le chemin trouvé ou null (si pas trouvé)
        //------------------------------------------------------------------------------
        {
            int level = 0;
            string curPath = "";
            bool trouve = false;

            do
            {
                //Console.WriteLine("RetrouverCheminFichier : " + curPath + FileName);

                if (File.Exists(curPath + FileName))
                {
                    //Console.WriteLine("oui !");
                    trouve = true;
                    break;
                }
                else
                {
                    //Console.WriteLine("non...");
                    curPath = "..\\" + curPath;     // remonter d'un niveau dans l'arborescence
                    level++;

                    if (level >= 10) break;
                }
            }
            while (!trouve);

            if (trouve)  return curPath;
            else         return null;
        }//-----------------------------------------------------------------------------

        static public void Ouvrir(string Chemin = "") //--------------------------------
        //
        //  Lancer l'application "EnigmAlgo.exe" si elle n'est pas déjà ouverte
        //  La durée de chargement doit être inférieure à 'AttenteMaxi' sinon
        //  le programma considère qu'il n'a pas pu charger "EnigmAlgo.exe"
        //------------------------------------------------------------------------------
        {
            if (Internal_Error) return;  // Sécurité !

            int AttenteMaxi = 5000; // mili-sec : Const 
            int StartTime;          //: LongWord;
            int ConsoleHandle;      //: THandle;

            //  ConsoleHandle := GetConsoleHwnd();

            //--- Vérifier que l'application soit déjà lancée
            RetrouverApplication(EnigmAlgo_WindowTitle);

            if (Callback_Found) Internal_AppHandle = Callback_Handle;
            else {
                //--- l'application n'est pas lancée : la démarrer !

                if (Chemin == "")
                {
                    //Chemin = EnigmAlgo_Dir;
                    Chemin = RetrouverCheminFichier("EnigmAlgo.exe");
                    //Console.Write("Chemin trouvé = " + Chemin);  Console.ReadLine();
                }

                DemarrerApplication(Chemin + "EnigmAlgo.exe");

                //    Attendre ( 1000 ) ;
                //--- Vérifier que l'application soit maintenant lancée
                StartTime = Environment.TickCount;
                do
                {   RetrouverApplication(EnigmAlgo_WindowTitle);

                    if (Callback_Found) break;
                    if ((Environment.TickCount - StartTime) >= AttenteMaxi) break;
                }
                while (true);

                if (Callback_Found) Internal_AppHandle = Callback_Handle;
                else
                    //--- l'application n'est toujours pas lancée : c'est une erreur !
                    Internal_Error = true;
            }
            /*
            Console.WriteLine("Debug : Callback_Option        = " + Callback_Option);
            Console.WriteLine("Debug : Callback_Title         = " + Callback_Title);
            Console.WriteLine("Debug : Callback_Handle        = " + Callback_Handle);
            Console.WriteLine("Debug : Callback_Found         = " + Callback_Found);
            Console.WriteLine("Debug : Internal_Error         = " + Internal_Error);
            Console.WriteLine("Debug : Internal_AppHandle     = " + Internal_AppHandle);
            Console.WriteLine("Debug : Internal_MessageResult = " + Internal_MessageResult);
            */
            //--- Passer la fenetre du jeu en premier plan
            if(!Internal_Error)   SetForegroundWindow((IntPtr)Internal_AppHandle);

            //--- Remettre le Focus sur la console
            //    Fait chier !!! y'a rien qui marche !!!
//            ConsoleHandle = GetConsoleHwnd();
//            SetForegroundWindow((IntPtr)ConsoleHandle);


            //--- Dimensionner et positionner proprement la console
            //Console.SetWindowPosition(1, 1);
            //SetWindowPos(handle, 0, 0, 0, form.Bounds.Width, form.Bounds.Height, SWP_NOZORDER | SWP_SHOWWINDOW);
            /*
            const short SWP_NOMOVE = 0X2;
            const short SWP_NOSIZE = 1;
            const short SWP_NOZORDER = 0X4;
            const int SWP_SHOWWINDOW = 0x0040;

            SetWindowPos(GetConsoleWindow(), 0, 0, 0, 60, 40, SWP_NOZORDER | SWP_SHOWWINDOW);
            */
            Console.SetWindowSize(60, 40);
            Attendre(500);
            SetForegroundWindow(GetConsoleWindow());
        }//-----------------------------------------------------------------------------
        static public void Fermer() //--------------------------------------------------
        //
        //  Fermer l'application "EnigmAlgo.exe" si elle est encore ouverte...
        //------------------------------------------------------------------------------
        {
            if (Internal_Error) return;  // Sécurité !

            //--- Fixer les paramètres pour EnumWindows
            Callback_Title  = EnigmAlgo_WindowTitle;   // Titre de la fenêtre recherchée
            Callback_Option = 'K';   // 'K'ill , 'F'ind
            Callback_Handle =  0 ;   // Handle de l'application trouvée
            Callback_Found  = false;   // Resultat booléen de la recherche
            CallBack myCallBack = new CallBack(Callback_EnumWindows);
            EnumWindows(myCallBack, 0);  // Enumérer ...

            //--- Vérifier que la fermeture ait été effectuée
            Attendre(500);
            RetrouverApplication(EnigmAlgo_WindowTitle);

            if (Callback_Found)
            {
                //--- l'application est toujours ouverte : c'est une erreur !
                Internal_Error = true;

//                Console.WriteLine("Fermeture ok");
            }
        }//--------------------------------------------------------------------------
        //==============================================================================
        //
        //  Chargement des Exercices dans l'Application
        //
        //==============================================================================
        static private IntPtr XSendMessage(WORD Msg, WORD wParam, LONGWORD lParam) // : LongInt ;
        //
        //  Envoyer un message à l'application
        //    <Msg>     message to send
        //    <wParam>  first  message parameter
        //    <lParam>  second message parameter
        //------------------------------------------------------------------------------
        {
            IntPtr Result = IntPtr.Zero;  // warning compilateur

            if (Internal_Error) return Result;  // Sécurité !
            if (Internal_AppHandle == 0) return Result;  // Sécurité !

            //  Internal_MessageResult := SendMessage ( Internal_AppHandle , Msg, wParam, lParam ) ;
            Result = SendMessage((IntPtr)Internal_AppHandle, Msg, wParam, (IntPtr)lParam);
            return Result;
        }//-----------------------------------------------------------------------------
        static private IntPtr XSendMessage4Bytes(WORD Msg, byte b1, byte b2,
                                byte b3 = 0,
                                byte b4 = 0,
                                byte b5 = 0,
                                byte b6 = 0) // : LongInt ; //-------------------
        //
        //  Envoyer un message à l'application
        //    <Msg>     message to send
        //    <wParam>  first  message parameter : b1 b2
        //    <lParam>  second message parameter : b3 b4 b5 b6
        //
        //  |      WPARAM       |  LPARAM                  |
        //  | 1 octet | 1 octet |  4 octets                |
        //  | Option  |  Action |  paramètres-de-l-action  |
        //------------------------------------------------------------------------------
        {
            int wParam; // WORD
            int lParam; //: LongWord;
            IntPtr Result = IntPtr.Zero;  // warning compilateur

            if (Internal_Error) return Result;  // Sécurité !
            if (Internal_AppHandle == 0) return Result;  // Sécurité !

            wParam = (b1 << 8) | b2;
            lParam = (b3 << 24) | (b4 << 16) | (b5 << 8) | b6;

            Result = XSendMessage(Msg, (WORD)wParam, (LONGWORD)lParam);
            return Result;
        }//-----------------------------------------------------------------------------
        static private IntPtr XSendMessage2Words(WORD Msg, byte b1, byte b2,
                                        WORD w1 = 0,
                                        WORD w2 = 0)// : LongInt ; //-------------------
        //
        //  Envoyer un message à l'application
        //    <Msg>     message to send
        //    <wParam>  first  message parameter : b1 b2
        //    <lParam>  second message parameter : w1 w2
        //
        //  |      WPARAM       |  LPARAM                  |
        //  | 1 octet | 1 octet |  4 octets                |
        //  | Option  |  Action |  paramètres-de-l-action  |
        //------------------------------------------------------------------------------
        {
            int wParam;//word
            int lParam; //: LongWord;
            IntPtr Result = IntPtr.Zero;  // warning compilateur

            if (Internal_Error) return Result;  // Sécurité !
            if (Internal_AppHandle == 0) return Result;  // Sécurité !

            wParam = (b1 << 8) | b2;
            lParam = (w1 << 16) | w2;

            Result = XSendMessage(Msg, (WORD)wParam, (LONGWORD)lParam);
            return Result;
        }//--------------------------------------------------------------------------
        static private void AttendreFinAction() //--------------------------------------
        //
        //  Attendre la fin de l'action en cours.
        //------------------------------------------------------------------------------
        {
            do
            {
                Attendre(300);
                Internal_MessageResult = XSendMessage4Bytes(WM_USER, (byte)TMessageOption._Consulter,
                                                                     (byte)TMessageAction._AucuneAction);
            }
            while (!(Internal_MessageResult == (IntPtr)1));
        }//-----------------------------------------------------------------------------
        static public void ChargerExo(int NumExo) //------------------------------------
        //
        //  Charger l'exercice <NumExo> dans l'application "EnigmAlgo.exe" si elle est
        //  ouverte.
        //------------------------------------------------------------------------------
        {
            if (Internal_Error) return;  // Sécurité !

            Internal_MessageResult = XSendMessage2Words(WM_USER, (byte)TMessageOption._Effectuer,
                                                                 (byte)TMessageAction._ChargerExo,
                                                                 (WORD)NumExo);
            AttendreFinAction();
        }//-----------------------------------------------------------------------------
        //==============================================================================
        //
        //  Manipulation de l'Acteur
        //
        //==============================================================================
        static public void Avancer() //---------------------------------------------------------
        //
        //  Déplacer l'Acteur d'une case dans la direction actuelle
        //------------------------------------------------------------------------------
        {
            if (Internal_Error) return;  // Sécurité !


            Internal_MessageResult = XSendMessage4Bytes(WM_USER, (byte)TMessageOption._Effectuer, 
                                                                (byte)TMessageAction._ActeurAvancer);

            AttendreFinAction();
        }//--------------------------------------------------------------------------
        static public bool PeutAvancer() //---------------------------------------------
        //
        //  Renvoyer Vrai si l'acteur peut avancer d'une case
        //------------------------------------------------------------------------------
        {
            bool Result = false;  // Warning compilateur

            if (Internal_Error) return Result;  // Sécurité !
            Internal_MessageResult = XSendMessage4Bytes(WM_USER, (byte)TMessageOption._Possible, (byte)TMessageAction._ActeurAvancer );
            Result = (Internal_MessageResult == (IntPtr)1) ;
            return Result;
        }//--------------------------------------------------------------------------
        static public void TournerADroite() //-----------------------------------------------
        //
        //  Faire tourner l'Acteur à Droite de 90°
        //------------------------------------------------------------------------------
        {
            if (Internal_Error) return;  // Sécurité !

            Internal_MessageResult = XSendMessage4Bytes(WM_USER,
                                                 (byte)TMessageOption._Effectuer,
                                                 (byte)TMessageAction._ActeurTournerDroite );
            AttendreFinAction();
        }//--------------------------------------------------------------------------
        static public void TournerAGauche() //------------------------------------------
        //
        //  Faire tourner l'Acteur à Gauche de 90°
        //------------------------------------------------------------------------------
        {
            if (Internal_Error) return;  // Sécurité !

            Internal_MessageResult = XSendMessage4Bytes(WM_USER,
                                                 (byte)TMessageOption._Effectuer,
                                                 (byte)TMessageAction._ActeurTournerGauche);
            AttendreFinAction();
        }//-----------------------------------------------------------------------------
        static public int Direction() //------------------------------------------------
        //
        //  Renvoyer le numéro de la direction de l'acteur 0:nord 1:est 2:sud 3:est
        //------------------------------------------------------------------------------
        {
            int Result = -1;  // Par défaut ... ???
            
            if (Internal_Error) return Result;  // Sécurité !

            Internal_MessageResult = XSendMessage4Bytes(WM_USER,
                                                       (byte)TMessageOption._Consulter,
                                                       (byte)TMessageAction._ActeurGetDirection );
            Result = (int)Internal_MessageResult / 4 ;
            return Result;
        }//-----------------------------------------------------------------------------
        static public void Pousser() //-------------------------------------------------
        //
        //  Pousser l'objet qui se trouve devant l'Acteur (si cet objet existe)
        //------------------------------------------------------------------------------
        {
            if (Internal_Error) return;  // Sécurité !
        }//-----------------------------------------------------------------------------
        static public bool MissionEstTerminee() //--------------------------------------
        //
        //  Renvoyer Vrai si la mission est terminée (gagnée ou perdue)
        //------------------------------------------------------------------------------
        {
            bool Result = false;  // warning compilateur

            if (Internal_Error) return Result;  // Sécurité !

            Internal_MessageResult = XSendMessage4Bytes(WM_USER, (byte)TMessageOption._Consulter, 
                                                              (byte)TMessageAction._MissionEstTerminee );

            Result = (Internal_MessageResult != IntPtr.Zero) ;
            return Result;
        }//-----------------------------------------------------------------------------
        static public bool MissionEstGagnee() //----------------------------------------
        //
        //  Renvoyer Vrai si le but de la mission est atteint (gagné)
        //------------------------------------------------------------------------------
        {
            bool Result = false;  // warning compilateur

            if (Internal_Error) return Result;  // Sécurité !
  
            Internal_MessageResult = XSendMessage4Bytes(WM_USER, (byte)TMessageOption._Consulter, 
                                                                (byte)TMessageAction._MissionEstTerminee);

            Result = (Internal_MessageResult == (IntPtr)(+1)) ;
            return Result;
        }//-----------------------------------------------------------------------------
        static public bool MissionEstPerdue() //----------------------------------------
        //
        //  Renvoyer Vrai si la mission perdue
        //------------------------------------------------------------------------------
        {
            bool Result = false;  // warning compilateur

            if (Internal_Error) return Result;  // Sécurité !

            Internal_MessageResult = XSendMessage4Bytes(WM_USER, (byte)TMessageOption._Consulter, 
                                                                (byte)TMessageAction._MissionEstTerminee);

            Result = (Internal_MessageResult == (IntPtr)(-1)) ;
            return Result;
        }//-----------------------------------------------------------------------------
    }
}
