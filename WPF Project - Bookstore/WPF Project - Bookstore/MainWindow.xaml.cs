using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    
    public partial class MainWindow : Window
    {
        
        public MainWindow()
        {
            InitializeComponent();
        }
    }
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
        string name;
        string author;
        Genre genre;
        string printNo;
        public Book(string name, string author, Genre genre, string printno)
        {
            this.name = name;
            this.author = author;
            this.genre = genre;
            this.printNo = printno;
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
        string username;
        string password;
        public Manager(string username , string password )
        {
            this.username = username;
            this.password = password;
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
        string username;
        string password;
        public Employee(string username, string password)
        {
            this.username = username;
            this.password = password;
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
    class Member : IShow,IChange
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
        public void returnLoanedbook( string bookname)
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
