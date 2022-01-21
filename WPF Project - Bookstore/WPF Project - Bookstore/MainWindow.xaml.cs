using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF_Project___Bookstore
{

    // tu tab item Bank Account ye jayi hast ke bayad mojudi ro be karbar neshun bedi ---> zamani k tab item dare initialize mishe un textblock ro meghdar dehi kon
    // (barat tu front comment gozashtam balash neveshtam "namayeshe mojudi"  Esme TextBlock : Manager_BankAccount_BankBalance  )
    // har ja dari tab iteme bank accounto baz mikoni ---> Manager_BankAccount_BankBalance.Text ro mosavie mojudi gharar bede 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        // Mouse Captured Change : 

        // *** : har moghe karbar zad rushun textesh khalishe : ino codesho barat mifrestam ke chejuri in karo koni
        // *** : baraye hameye mouse capture ha bayad motenaseb ba range bordere zireshun , range border ro yekam porrang tar koni : inam nemune codesho mifrestam vali rangesho bayad khodet ok koni 


        // login page : 
        private void LoginUsernameBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            //LoginUsernameBorder.Background = Brushes.Black;
            //LoginUsernameBox.Text = "";
        }
        private void LoginPasswordBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }


        // register page : 
        private void RegisterPasswordBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void RegisterUsernameBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void RegisterTelephoneNumberBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void RegisterEmailBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        // payment page : 
        private void CreditCardNumberBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            
        }
        private void CVV2Box_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void monthBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void yearbox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        // manager bank acconut section : 
        private void Manager_BankAccount_DepositMoney_TextBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        // Manager Add Books Section : 
        private void AddBooks_PrintNoBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void AddBooks_BookNameBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void AddBooks_GenreBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void AddBooks_AuthorBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }





        // Button Click

        // Back Button : hamishe tab itemi k tush hastim ro mibande 
        // age be tor koli mituni back itemo piade sazi koni tori ke har dafe tab itemi ke tush hastim ro bebande o tab iteme ghablio baz kone ke hich
        // vali age natunesti Button hayi k man in payin comment kardam o az comment dar biar o boro tu fronte har kudum esmeshuno dorost kon (felan hamashun ba esme Back_Button_Click hastan) 

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            
        }






        // Login Page  : 

        private void Login_SignIn_Button_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Login_SignUp_Button_Click(object sender, RoutedEventArgs e)
        {

        }







        // Register Page  : 

        private void Register_SignUp_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        //private void Register_BackButton_Click(object sender , RoutedEventArgs e)
        //{

        //}





        // Payment Page : 

        private void Payment_PayButton_Click(object sender, RoutedEventArgs e)
        {

        }

        //private void Payment_BackButton_Click(object sender, RoutedEventArgs e)
        //{

        //}







        // Manager Main Menu Page :

        private void Manager_EmplpyeeButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Manager_BookButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Manager_BankAccountButton_Click(object sender, RoutedEventArgs e)
        {

        }

        //private void Manager_MainMenu_BackButton_Click(object sender, RoutedEventArgs e)
        //{

        //}








        // Manager Employee Section : 
        private void Manager_EmployeeSection_ListSection_AddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Manager_EmployeeSection_ListSection_PayButton_Click(object sender, RoutedEventArgs e)
        {

        }
        //private void Manager_EmployeeSection_ListSection_Back_Button_Click(object sender, RoutedEventArgs e)
        //{

        //}






        // Manager Book Section : 

        private void Manager_BooksSection_AddButton_Click(object sender, RoutedEventArgs e)
        {

        }
        //private void Manager_BooksSection_BackButton_Click(object sender, RoutedEventArgs e)
        //{

        //}







        // Manager Bank Account Section : 
        private void Manager_BankAccountSection_DepositButton_Click(object sender, RoutedEventArgs e)
        {

        }
        //private void Manager_BankAccountSection_BackButton_Click(object sender, RoutedEventArgs e)
        //{

        //}







        private void AddBooks_AddButton_Click(object sender, RoutedEventArgs e)
        {

        }
        //private void AddBooks_BackButton_Click(object sender, RoutedEventArgs e)
        //{
                
        //}



    }








    // fek konam baraye ghesmate data grid bayad field ha hame be surate property bashan --> property haro taarif kardam , baraye data grid az una estefade kon
    // kolan hameja az property estefade kon

    public enum Genre
    {
        Classic,
        ComicBook,
        Fantasy,
        Mystery,
        Romance,
        ScienceFiction
    }
    public class Book
    {
        string _name;
        string _author;
        Genre _genre;
        string _printNo;

        public string Name
        {
            get { return _name; }
            set { this._name = value; }
        }
        public string Author
        {
            get { return _author; }
            set { this._author = value; }
        }
        public Genre genre
        {
            get { return _genre; }
            set { this._genre = value; }
        }
        public string PrintNo
        {
            get { return this._printNo; }
            set { this._printNo = value; }
        }
        public Book(string name, string author, Genre genre, string printno)
        {
            this._name = name;
            this._author = author;
            this._genre = genre;
            this._printNo = printno;
        }
    }
    interface IShow
    {
        void showBooklist();
    }
    interface IChange
    {
        void changePersonalInforamtion();
    }
    class Manager : IShow
    {
        string _username;
        string _password;

        public string Username
        {
            get { return this._username; }
            set { this._username = value; }
        }
        public string Password
        { 
            get { return this._password; }
            set { this._password = value; }
        }

        public Manager(string username, string password)
        {
            this._username = username;
            this._password = password;
        }
        public void hireEmployee(int id)
        {

        }
        public void fireEmployee(int id)
        {

        }
        public void payEmployee()
        {

        }
        public void addBook(Book newbook)
        {

        }
        public void showBooklist()
        {

        }
        public void showBookstoreBalance()
        {

        }
        public void depositBookstore()
        {

        }
    }
    class Employee : IShow, IChange
    {
        string _username;
        string _password;

        public string Username
        {
            get { return this._username; }
            set { this._username = value; }
        }
        public string Password
        {
            get { return this._password; }
            set { this._password = value; }
        }
        public Employee(string username, string password)
        {
            this._username = username;
            this._password = password;
        }
        public void showBorrowedBooklist()
        {

        }
        public void showUnborrowedBooklist()
        {

        }
        public void showLibraryMembers()
        {

        }
        public void showTardyMemberInReturningLoanedBook()
        {

        }
        public void showTardyMemberInPayMonthlyMembership()
        {

        }
        public void showSpecifiedMemberState()
        {

        }
        public void showPersonalAccountBalance()
        {

        }
        public void changePersonalInforamtion()
        {

        }
        public void showBooklist()
        {

        }
    }
    class Member : IShow, IChange
    {
        string username;
        string password;
        string emailAddress;
        string telephoneNumber;
        public Member(string username, string password, string emailAddress, string telephonenumber)
        {
            this.username = username;
            this.password = password;
            this.emailAddress = emailAddress;
            this.telephoneNumber = telephonenumber;
        }
        public void loanBook(string bookname)
        {

        }
        public void returnLoanedbook(string bookname)
        {

        }
        public void payLateReturnPenalty()
        {

        }
        public void payMonthlyMembershipCost()
        {

        }
        public void showPersonalAccountBalance()
        {

        }
        public void depositPersonalAccount()
        {

        }
        public void changePersonalInforamtion()
        {

        }
        public void showBooklist()
        {

        }
    }


}
