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
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Collections.ObjectModel;

namespace WPF_Project___Bookstore
{
    enum Types { Admin = 1, Employee, Member, NotFound, NotMatched, Bug }
    // tu tab item Bank Account ye jayi hast ke bayad mojudi ro be karbar neshun bedi ---> zamani k tab item dare initialize mishe un textblock ro meghdar dehi kon
    // (barat tu front comment gozashtam balash neveshtam "namayeshe mojudi"  Esme TextBlock : Manager_BankAccount_BankBalance  )
    // har ja dari tab iteme bank accounto baz mikoni ---> Manager_BankAccount_BankBalance.Text ro mosavie mojudi gharar bede 
    public partial class MainWindow : Window
    {
        //Temps
        string temp_username, temp_email, temp_phone_number, temp_password;
        Types Access = Types.NotFound;
        Manager TempManager;
        Employee TempEmployee;
        Member TempMember;
        Book TempBook;
        public List<string> RemoveEmployees = new List<string>();
        public ObservableCollection<Employee> Employees { get; set; } = new ObservableCollection<Employee>();
        public ObservableCollection<Book> Books { get; set; } = new ObservableCollection<Book>();
        public MainWindow()
        {
            InitializeComponent();
            foreach (TabItem tab in Tabs.Items)
            {
                //tab.Header = null;
                //tab.Visibility = Visibility.Hidden;
            }
        }

        //Database Functions
        private Types usernameAvailableAndMatch(string u,string p)
        {
            bool flagexist = false;
            bool flagmatch = false;
            string type = "";
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            command = "select * from users";
            SqlDataAdapter adapter = new SqlDataAdapter(command, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if ((string)dataTable.Rows[i][1] == u.ToLower())
                {
                    type = (string)dataTable.Rows[i][5];
                    flagexist = true;
                    if (p == (string)dataTable.Rows[i][4])
                    {
                        flagmatch = true;
                    }
                    break;
                }
                Console.WriteLine(dataTable.Rows[i][0]);
            }
            SqlCommand comm = new SqlCommand(command, conn);
            comm.BeginExecuteNonQuery();
            conn.Close();
            if (flagexist)
            {
                if (flagmatch)
                {
                    if (type == "admin")
                    {
                        return Types.Admin;
                    }
                    else if (type == "employee")
                    {
                        return Types.Employee;
                    }
                    else if (type == "member")
                    {
                        return Types.Member;
                    }
                    else
                    {
                        return Types.Bug;
                    }
                }
                else
                {
                    return Types.NotMatched;
                }
            }
            else
            {
                return Types.NotFound;
            }
        }
        private bool usernameNotTaken(string u)
        {
            bool flagnotexist = true;
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            command = "select username from users";
            SqlDataAdapter adapter = new SqlDataAdapter(command, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (u == (string)dataTable.Rows[i][0])
                {
                    flagnotexist = false;
                }
            }
            return flagnotexist;
        }
        private bool emailNotTaken(string e)
        {
            bool flagnotexist = true;
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            command = "select email from users";
            SqlDataAdapter adapter = new SqlDataAdapter(command, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (e == (string)dataTable.Rows[i][0])
                {
                    flagnotexist = false;
                }
            }
            return flagnotexist;
        }
        private bool phoneNumberNotTaken(string pn)
        {
            bool flagnotexist = true;
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            command = "select phone_number from users";
            SqlDataAdapter adapter = new SqlDataAdapter(command, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (pn == (string)dataTable.Rows[i][0])
                {
                    flagnotexist = false;
                }
            }
            return flagnotexist;
        }


        //Regex
        private bool UserNameRegex(string name)
        {
            string strRegex = @"(^[a-z A-Z]{3,32}$)";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(name))
                return (true);
            else
                return (false);
        }
        private bool EmailRegex(string name)
        {
            string strRegex = @"(^[a-z A-Z 0-9 _ -]{1,32}@[a-z A-Z 0-9]{1,8}.[a-z A-Z]{1,3}$)";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(name))
                return (true);
            else
                return (false);
        }
        private bool PhoneNumberRegex(string name)
        {
            string strRegex = @"(^09[0-9]{9}$)";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(name))
                return (true);
            else
                return (false);
        }
        private bool PasswordRegex(string name)
        {
            string strRegex1 = @"([a-z A-Z]{8,32})";
            string strRegex2 = @"([A-Z]+)";
            Regex re1 = new Regex(strRegex1);
            Regex re2 = new Regex(strRegex2);
            if (re1.IsMatch(name) && re2.IsMatch(name))
                return (true);
            else
                return (false);
        }
        private bool CVVRegex(string name)
        {
            string strRegex = @"(^[0-9]{3,4}$)";
            Regex re = new Regex(strRegex);
            if (re.IsMatch(name))
                return (true);
            else
                return (false);
        }
        private bool CardNumberRegex(string name)
        {
            if(name.Length != 16)
            {
                return false;
            }
            int[] digits = new int[16];
            int sum = 0;
            for(int i = 0; i < 16; i+=2)
            {
                sum += (int.Parse(name[i].ToString()) * 2) % 10;
                sum += (int)((int.Parse(name[i].ToString()) * 2) / 10);
                sum += int.Parse(name[i + 1].ToString());
            }
            if (sum % 10 != 0)
            {
                return false;
            }
            return true;
        }
        private bool MonthRegex(string year,string month)
        {
            try
            {
                if (int.Parse(month) > 12 || int.Parse(month) < 1 || int.Parse(year) < 1)
                {
                    return false;
                }
                DateTime currentDateTime = DateTime.Today;
                int sumCurrentMonth = currentDateTime.Year * 12 + currentDateTime.Month;
                int sumUserMonth = int.Parse(year) * 12 + int.Parse(month);
                if (sumUserMonth - sumCurrentMonth < 3)
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }


        // login page : 
        private void Login_SignIn_Button_Click(object sender, RoutedEventArgs e)
        {
            bool flag = true;
            if (LoginUsernameBox.Text != "")
            {
                Login_Usarname_Alert.Text = "";
            }
            else
            {
                Login_Usarname_Alert.Text = "Username can't be empty!";
                flag = false;
            }
            if (LoginPasswordBox.Password != "")
            {
                Login_Password_Alert.Text = "";
            }
            else
            {
                Login_Password_Alert.Text = "Password can't be empty!";
                flag = false;
            }
            try
            {
                if (flag)
                {
                    switch (usernameAvailableAndMatch(LoginUsernameBox.Text, LoginPasswordBox.Password))
                    {
                        case Types.Admin:
                            Access = Types.Admin;
                            TempManager = new Manager();
                            TempBook = new Book();
                            if (TempManager.fillUserWith(LoginUsernameBox.Text) == false)
                            {
                                throw new Exception("Database Error!!!");
                            }
                            LoginUsernameBox.Text = "";
                            LoginPasswordBox.Password = "";


                            Employees = TempManager.getEmployees();
                            Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

                            Books = TempBook.getBooks();
                            Bindi_Books.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

                            DataContext = this;
                            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Main_Menu_Page));

                            Manager_Balance.Text = "Balance : " + TempManager.Balance + " T";
                            break;
                        case Types.Employee:
                            Access = Types.Employee;
                            TempEmployee = new Employee();
                            if (TempEmployee.fillUserWith(LoginUsernameBox.Text) == false)
                            {
                                throw new Exception("Database Error!!!");
                            }
                            LoginUsernameBox.Text = "";
                            LoginPasswordBox.Password = "";
                            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Employee_Main_Menu_Page));
                            break;
                        case Types.Member:
                            Access = Types.Member;
                            TempMember = new Member();
                            if (TempMember.fillUserWith(LoginUsernameBox.Text) == false)
                            {
                                throw new Exception("Database Error!!!");
                            }
                            LoginUsernameBox.Text = "";
                            LoginPasswordBox.Password = "";
                            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Main_Menu_Page));
                            break;
                        case Types.NotFound:
                            Access = Types.NotFound;
                            Login_Usarname_Alert.Text = "Username not found!";
                            break;
                        case Types.NotMatched:
                            Access = Types.NotMatched;
                            Login_Password_Alert.Text = "Password is wrong!";
                            break;
                        case Types.Bug:
                            Access = Types.Bug;
                            Login_Usarname_Alert.Text = "Bug!";
                            break;
                        default:
                            Access = Types.Bug;
                            Login_Usarname_Alert.Text = "Bug!";
                            break;
                    }
                }
            }
            catch(Exception ee)
            {
                Login_Usarname_Alert.Text = ee.Message;
            }
        }
        private void Login_SignUp_Button_Click(object sender, RoutedEventArgs e)
        {
            Login_Password_Alert.Text = "";
            Login_Usarname_Alert.Text = "";
            LoginUsernameBox.Text = "";
            LoginPasswordBox.Password = "";
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedIndex = Register_Page.TabIndex));
        }
















        // register page :
        private void Register_SignUp_Button_Click(object sender, RoutedEventArgs e)
        {
            bool flag = true;
            if (UserNameRegex(RegisterUsernameBox.Text))
            {
                if (usernameNotTaken(RegisterUsernameBox.Text))
                {
                    Register_Usarname_Alert.Text = "";
                }
                else
                {
                    Register_Usarname_Alert.Text = "Username has already taken!";
                    flag = false;
                }
            }
            else
            {
                Register_Usarname_Alert.Text = "Username does not have the correct format!";
                flag = false;
            }
            if (EmailRegex(RegisterEmailBox.Text))
            {
                if (emailNotTaken(RegisterEmailBox.Text))
                {
                    Register_Email_Alert.Text = "";
                }
                else
                {
                    Register_Email_Alert.Text = "Email has already taken!";
                    flag = false;
                }
            }
            else
            {
                Register_Email_Alert.Text = "Email does not have the correct format!";
                flag = false;
            }
            if (PhoneNumberRegex(RegisterPhoneNumberBox.Text))
            {
                if (phoneNumberNotTaken(RegisterPhoneNumberBox.Text))
                {
                    Register_PhoneNumber_Alert.Text = "";
                }
                else
                {
                    Register_PhoneNumber_Alert.Text = "Password has already taken!";
                    flag = false;
                }
            }
            else
            {
                Register_PhoneNumber_Alert.Text = "Phone Number does not have the correct format!";
                flag = false;
            }
            if (PasswordRegex(RegisterPasswordBox.Password))
            {
                Register_Password_Alert.Text = "";
            }
            else
            {
                Register_Password_Alert.Text = "Password does not have the correct format!";
                flag = false;
            }
            if (flag)
            {
                temp_username = RegisterUsernameBox.Text;
                temp_email = RegisterEmailBox.Text;
                temp_phone_number = RegisterPhoneNumberBox.Text;
                temp_password = RegisterPasswordBox.Password;
                RegisterUsernameBox.Text = "";
                RegisterEmailBox.Text = "";
                RegisterPhoneNumberBox.Text = "";
                RegisterPasswordBox.Password = "";
                Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Payment_Page));
            }
        }

        private void Register_SignIn_Click(object sender, RoutedEventArgs e)
        {
            Register_Usarname_Alert.Text = "";
            Register_Email_Alert.Text = "";
            Register_PhoneNumber_Alert.Text = "";
            Register_Password_Alert.Text = "";
            RegisterUsernameBox.Text = "";
            RegisterEmailBox.Text = "";
            RegisterPhoneNumberBox.Text = "";
            RegisterPasswordBox.Password = "";
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Login_Page));
        }














        // payment page :
        private void Payment_PayButton_Click(object sender, RoutedEventArgs e)
        {
            bool flag = true;
            if (CardNumberRegex(CreditCardNumberBox.Text))
            {
                Payment_CardNumber_Alert.Text = "";
            }
            else
            {
                Payment_CardNumber_Alert.Text = "Card Number does not have the correct format!";
                flag = false;
            }
            if (CVVRegex(CVV2Box.Text))
            {
                Payment_CVV_Alert.Text = "";
            }
            else
            {
                Payment_CVV_Alert.Text = "Cvv2 does not have the correct format!";
                flag = false;
            }
            if (MonthRegex(YearBox.Text, monthBox.Text))
            {
                Payment_Month_Alert.Text = "";
            }
            else
            {
                Payment_Month_Alert.Text = "Your card has expired or is not formatted correctly!";
                flag = false;
            }
            if (flag)
            {
                CreditCardNumberBox.Text = "";
                CVV2Box.Text = "";
                monthBox.Text = "";
                YearBox.Text = "";
                if (Access == Types.Admin)
                {
                    //TempManager
                }
                if (Access == Types.NotFound)
                {
                    TempMember = new Member(temp_username.ToLower(), temp_email.ToLower(), temp_phone_number, temp_password, "member", 0);
                    if (TempMember.fillDatabase())
                    {
                        Access = Types.Member;
                        Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Main_Menu_Page));
                    }
                    else
                    {
                        Payment_Month_Alert.Text = "Database Bug !!!!";
                    }
                }
            }
        }
        private void Payment_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Payment_CardNumber_Alert.Text = "";
            Payment_CVV_Alert.Text = "";
            Payment_Month_Alert.Text = "";
            CreditCardNumberBox.Text = "";
            CVV2Box.Text = "";
            monthBox.Text = "";
            YearBox.Text = "";
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Register_Page));
        }







        // Manager Main Menu Page :

        private void Manager_EmplpyeeButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Employee_Section_List_Page));
        }

        private void Manager_BookButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Books_Sections_List_Page));
        }

        private void Manager_BankAccountButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Bank_Account_Section_Page));
        }






        // Manager Employee Section :
        private void Manager_EmployeeSection_ListSection_AddButton_Click(object sender, RoutedEventArgs e)
        {
            Remove_All_Selected_Employee_Button.Visibility = Visibility.Hidden;
            Employees = TempManager.getEmployees();
            RemoveEmployees = new List<string>();
            Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Add_Employee_Menu_Page));
        }

        private void Manager_EmployeeSection_ListSection_PayButton_Click(object sender, RoutedEventArgs e)
        {
            
        }
        private void Employees_Sumbmit_Pay_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Manager_EmployeeSection_ListSection_Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Remove_All_Selected_Employee_Button.Visibility = Visibility.Hidden;
            Employees = TempManager.getEmployees();
            RemoveEmployees = new List<string>();
            Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Main_Menu_Page));
        }
        private void Remove_Employee_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Width == 10)
            {
                ((Button)sender).Width = 10.5;
                ((Button)sender).Height = 10.5;
                ((Button)sender).Background = Brushes.DeepPink.Clone();
                RemoveEmployees.Add(((Button)sender).Tag.ToString());
                Remove_All_Selected_Employee_Button.Visibility = Visibility.Visible;
            }
            else
            {
                ((Button)sender).Width = 10;
                ((Button)sender).Height = 10;
                ((Button)sender).Background = Brushes.YellowGreen.Clone();
                RemoveEmployees.Remove(((Button)sender).Tag.ToString());
                if (RemoveEmployees.Count == 0)
                {
                    Remove_All_Selected_Employee_Button.Visibility = Visibility.Hidden;
                }
            }
            
        }
        private void Remove_Employee_Click2(object sender, RoutedEventArgs e)
        {
            TempManager.removeSelectedEmployees(RemoveEmployees);
            RemoveEmployees = new List<string>();
            Employees = TempManager.getEmployees();
            Remove_All_Selected_Employee_Button.Visibility = Visibility.Hidden;
            Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
        }













        // Manager Book Section :

        private void Manager_BooksSection_AddButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Add_Book_Menu_Page));
        }
        private void Manager_BooksSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Main_Menu_Page));
        }







        // manager bank acconut section :
        private void Manager_BankAccountSection_DepositButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Payment_Page));
        }
        private void Manager_BankAccountSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Main_Menu_Page));
        }







        // Manager Add Employee Section :
        private void AddEmployees_PrintNoBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void AddEmployees_BookNameBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void AddEmployees_GenreBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void AddEmployees_AuthorBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }

        private void AddEmployees_AddButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void AddEmployees_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Employee_Section_List_Page));
        }






        // Manager Add Books Section :
        private void AddBooks_AddButton_Click(object sender, RoutedEventArgs e)
        {
            bool flag = true;
            if (AddBooks_BookNameBox.Text == "")
            {
                Manager_Add_Book_BookName_Alert.Text = "Name can't be empty!";
                flag = false;
            }
            else
            {
                Manager_Add_Book_BookName_Alert.Text = "";
                flag = true;
            }
            if (AddBooks_AuthorBox.Text == "")
            {
                Manager_Add_Book_Author_Alert.Text = "Author can't be empty!";
                flag = false;
            }
            else
            {
                Manager_Add_Book_Author_Alert.Text = "";
                flag = true;
            }
            if (AddBooks_GenreBox.Text == "")
            {
                Manager_Add_Book_Genre_Alert.Text = "Genre can't be empty!";
                flag = false;
            }
            else
            {
                Manager_Add_Book_Genre_Alert.Text = "";
                flag = true;
            }
            if (AddBooks_PrintNoBox.Text == "")
            {
                Manager_Add_Book_PrintNo_Alert.Foreground = Brushes.Red.Clone();
                Manager_Add_Book_PrintNo_Alert.Text = "Publish Number can't be empty!";
                flag = false;
            }
            else
            {
                Manager_Add_Book_PrintNo_Alert.Text = "";
                flag = true;
            }
            if (flag)
            {
                TempBook = new Book(AddBooks_BookNameBox.Text, AddBooks_AuthorBox.Text, AddBooks_GenreBox.Text, AddBooks_PrintNoBox.Text, 1);
                TempBook.fillDatabase();
                AddBooks_BookNameBox.Text = "";
                AddBooks_AuthorBox.Text = "";
                AddBooks_GenreBox.Text = "";
                AddBooks_PrintNoBox.Text = "";
                Manager_Add_Book_BookName_Alert.Text = "";
                Manager_Add_Book_Author_Alert.Text = "";
                Manager_Add_Book_Genre_Alert.Text = "";
                Manager_Add_Book_PrintNo_Alert.Text = "";
                Manager_Add_Book_PrintNo_Alert.Foreground = Brushes.Green.Clone();
                Manager_Add_Book_PrintNo_Alert.Text = "Book successfully added.";
                Books = TempBook.getBooks();
                Bindi_Books.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
                DataContext = this;
            }
        }
        private void AddBooks_BackButton_Click(object sender, RoutedEventArgs e)
        {
            AddBooks_BookNameBox.Text = "";
            AddBooks_AuthorBox.Text = "";
            AddBooks_GenreBox.Text = "";
            AddBooks_PrintNoBox.Text = "";
            Manager_Add_Book_BookName_Alert.Text = "";
            Manager_Add_Book_Author_Alert.Text = "";
            Manager_Add_Book_Genre_Alert.Text = "";
            Manager_Add_Book_PrintNo_Alert.Text = "";
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Books_Sections_List_Page));
        }







        //Employee Main Menu Page :

        private void Employee_MembersButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Employee_Member_Section_List_Page));
        }

        private void Employee_BookButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Employee_Books_Section_List_Page));
        }

        private void Employee_BankAccountButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Employee_Bank_Account_Section_Page));
        }
        private void Employee_EditInformation_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Employee_Edit_Information_Section_Page));
        }






        //Employee Member Section :
        private void Employee_MembersSection_ListSection_AddButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Employee_MembersSection_ListSection_PayButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Employee_MembersSection_ListSection_Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Employee_Main_Menu_Page));
        }












        // Employee Book Section : 

        private void Employee_BooksSection_AddButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Employee_BooksSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Employee_Main_Menu_Page));
        }







        // Employee bank acconut section : 
        private void Employee_BankAccount_DepositMoney_TextBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void Employee_BankAccountSection_DepositButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Employee_BankAccountSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Employee_Main_Menu_Page));
        }









        // Edit acconut section : 
        private void Edit_BankAccount_DepositMoney_TextBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void Edit_BankAccountSection_DepositButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Employee_EditAccountSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Employee_Main_Menu_Page));
        }







        // Member Information section : 
        private void MemberInformation_BankAccount_DepositMoney_TextBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void MemberInformation_BankAccountSection_DepositButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Employee_MemberInformationSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Employee_Member_Section_List_Page));
        }



























































        //Member Main Menu Page :

        private void Member_BooksButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Books_Section_List_Page));
        }

        private void Member_MyBookButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_MyBooks_Section_List_Page));
        }

        private void Member_LicenseButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_License_Section_Page));
        }
        private void Member_BankAccount_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Bank_Account_Section_Page));
        }
        private void Member_EditInformation_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Edit_Information_Section_Page));
        }






        //Member Books Section :
        private void Member_BooksSection_AddButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Member_BooksSection_ListSection_Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Main_Menu_Page));
        }












        // Member MyBooks Section : 

        private void Member_MyBooksSection_AddButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Member_BooksSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Main_Menu_Page));
        }







        // Member License section : 
        private void Member_License_DepositMoney_TextBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void Member_LicenseSection_DepositButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Member_License_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Member_LicenseSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Main_Menu_Page));
        }










        // Member Bank Account section : 
        private void Member_BankAccount_DepositMoney_TextBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void Member_BankAccountSection_DepositButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Member_Bank_AccountSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Main_Menu_Page));
        }









        // Member Edit acconut section : 
        private void MemberEdit_BankAccount_DepositMoney_TextBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void MemberEdit_BankAccountSection_DepositButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Member_EditAccountSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Main_Menu_Page));
        }







        // Borrow section : 
        private void Borrow_BankAccount_DepositMoney_TextBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void Borrow_BankAccountSection_DepositButton_Click(object sender, RoutedEventArgs e)
        {

        }
        //private void Employee_BankAccountSection_BackButton_Click(object sender, RoutedEventArgs e)
        //{

        //}












        // Return section : 
        private void Return_BankAccount_DepositMoney_TextBox_IsMouseCapturedChanged(object sender, DependencyPropertyChangedEventArgs e)
        {

        }
        private void Return_BankAccountSection_DepositButton_Click(object sender, RoutedEventArgs e)
        {

        }
        //private void Employee_BankAccountSection_BackButton_Click(object sender, RoutedEventArgs e)
        //{

        //}



        //BackButton 

        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            Employees = null;
            Books = null;
            Access = Types.NotFound;
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Login_Page));
        }
        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }










































    //classes
    // fek konam baraye ghesmate data grid bayad field ha hame be surate property bashan --> property haro taarif kardam , baraye data grid az una estefade kon
    // kolan hameja az property estefade kon
    interface IShow
    {
        void showBooklist();
    }
    interface IChange
    {
        void changePersonalInforamtion();
    }
    public class Book
    {
        string _name;
        string _author;
        string _genre;
        string _publish_no;
        int _number;
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
        public string Genre
        {
            get { return _genre; }
            set { this._genre = value; }
        }
        public string PublishNo
        {
            get { return this._publish_no; }
            set { this._publish_no = value; }
        }
        public int Number
        {
            get { return this._number; }
            set { this._number = value; }
        }
        public Book(string name, string author, string genre, string printno, int number)
        {
            this._name = name;
            this._author = author;
            this._genre = genre;
            this._publish_no = printno;
            this._number = number;
        }
        public Book()
        {

        }
        public bool fillDatabase()
        {
            try
            {
                SqlConnection conn1 = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
                conn1.Open();

                //Check Exist
                bool exist = false;
                int num = 0;
                string command = "select * from books";
                SqlDataAdapter adapter = new SqlDataAdapter(command, conn1);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if(this._name == dataTable.Rows[i][1].ToString())
                    {
                        exist = true;
                        num = int.Parse(dataTable.Rows[i][5].ToString());
                        break;
                    }
                }
                SqlCommand comm1 = new SqlCommand(command, conn1);
                comm1.ExecuteNonQuery();
                conn1.Close();

                if (exist)
                {
                    conn1.Open();
                    command = "update books SET number = '" + (num + 1) + "' where name = '" + this._name.Trim() + "'";
                    comm1 = new SqlCommand(command, conn1);
                    comm1.ExecuteNonQuery();
                    conn1.Close();
                }
                else
                {
                    //Find Last ID
                    conn1.Open();
                    int last_id = 1;
                    command = "select id from books";
                    adapter = new SqlDataAdapter(command, conn1);
                    dataTable = new DataTable();
                    adapter.Fill(dataTable);
                    if (dataTable.Rows.Count > 0)
                    {
                        last_id = int.Parse(dataTable.Rows[dataTable.Rows.Count - 1][0].ToString()) + 1;
                    }
                    comm1 = new SqlCommand(command, conn1);
                    comm1.ExecuteNonQuery();
                    conn1.Close();
                    //Insert Data
                    conn1.Open();
                    command = "insert into books values('" + last_id + "','" + this._name.Trim() + "','" + this._author.Trim() + "','" + this._genre.Trim() + "','" + this._publish_no.Trim() + "','" + 1 + "')";
                    comm1 = new SqlCommand(command, conn1);
                    comm1.ExecuteNonQuery();
                    conn1.Close();
                }

            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public ObservableCollection<Book> getBooks()
        {
            ObservableCollection<Book> Bks = new ObservableCollection<Book>();
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            command = "select * from books";
            SqlDataAdapter adapter = new SqlDataAdapter(command, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                Bks.Add(new Book(dataTable.Rows[i][1].ToString(),
                                      dataTable.Rows[i][2].ToString(),
                                      dataTable.Rows[i][3].ToString(),
                                      dataTable.Rows[i][4].ToString(),
                                      int.Parse(dataTable.Rows[i][5].ToString())
                                      ));
            }
            SqlCommand comm = new SqlCommand(command, conn);
            comm.BeginExecuteNonQuery();
            conn.Close();
            return Bks;
        }
    }
    public class User
    {
        private string username;
        private string email;
        private string phone_number;
        private string password;
        private string type;
        private int balance;
        public string Username
        {
            get { return this.username; }
            set { this.username = value; }
        }
        public string Email
        {
            get { return this.email; }
            set { this.email = value; }
        }
        public string Phone_Number
        {
            get { return this.phone_number; }
            set { this.phone_number = value; }
        }
        public string Password
        {
            get { return this.password; }
            set { this.password = value; }
        }
        public string Type
        {
            get { return this.type; }
            set { this.type = value; }
        }
        public int Balance
        {
            get { return this.balance; }
            set { this.balance = value; }
        }
        public User(string username, string emailAddress, string telephonenumber, string password, string type , int balance)
        {
            this.username = username;
            this.email = emailAddress;
            this.phone_number = telephonenumber;
            this.password = password;
            this.type = type;
            this.balance = balance;
        }
        public User()
        {

        }
        public bool fillDatabase()
        {
            try
            {
                SqlConnection conn1 = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
                conn1.Open();
                //Find Last ID
                int last_id = 1;
                string command;
                command = "select id from users";
                SqlDataAdapter adapter = new SqlDataAdapter(command, conn1);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    last_id = int.Parse(dataTable.Rows[dataTable.Rows.Count - 1][0].ToString()) + 1;
                }
                SqlCommand comm1 = new SqlCommand(command, conn1);
                comm1.BeginExecuteNonQuery();
                conn1.Close();

                //Insert Data
                SqlConnection conn2 = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
                conn2.Open();
                command = "insert into Users values('" + last_id + "','" + this.username.Trim() + "','" + this.email.Trim() + "','" + this.phone_number.Trim() + "','" + this.password.Trim() + "','" + type.Trim() + "'),'" + this.balance + "')";
                SqlCommand comm2 = new SqlCommand(command, conn2);
                comm2.ExecuteNonQuery();
                conn2.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public bool fillUserWith(string username)
        {
            try
            {
                SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
                conn.Open();
                string command;
                command = "select * from users";
                SqlDataAdapter adapter = new SqlDataAdapter(command, conn);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (dataTable.Rows[i][1].ToString() == username)
                    {
                        this.username = dataTable.Rows[i][1].ToString();
                        this.email = dataTable.Rows[i][2].ToString();
                        this.phone_number = dataTable.Rows[i][3].ToString();
                        this.password = dataTable.Rows[i][4].ToString();
                        this.type = dataTable.Rows[i][5].ToString();
                        this.balance = int.Parse(dataTable.Rows[i][6].ToString());
                    }
                }
                SqlCommand comm = new SqlCommand(command, conn);
                comm.BeginExecuteNonQuery();
                conn.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
    }
    public class Manager : User
    {

        public Manager(string username, string emailAddress, string telephonenumber, string password, string type , int balance) : base(username,emailAddress,telephonenumber,password,type,balance)
        {

        }
        public Manager()
        {

        }
        public ObservableCollection<Employee> getEmployees()
        {
            ObservableCollection<Employee> Emps = new ObservableCollection<Employee>();
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            command = "select * from users";
            SqlDataAdapter adapter = new SqlDataAdapter(command, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i][5].ToString() == "employee")
                {
                    Emps.Add(new Employee(dataTable.Rows[i][1].ToString(),
                                          dataTable.Rows[i][2].ToString(),
                                          dataTable.Rows[i][3].ToString(),
                                          dataTable.Rows[i][4].ToString(),
                                          dataTable.Rows[i][5].ToString(),
                                          int.Parse(dataTable.Rows[i][6].ToString())));
                }
            }
            SqlCommand comm = new SqlCommand(command, conn);
            comm.BeginExecuteNonQuery();
            conn.Close();
            return Emps;
        }
        public void removeSelectedEmployees(List<string> emps)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            for(int i = 0; i < emps.Count; i++)
            {
                command = "delete from users where username = '" + emps[i] + "' ";
                SqlCommand comm = new SqlCommand(command, conn);
                comm.ExecuteNonQuery();
            }
            conn.Close();
        }
        public string payEmployee(int money)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            //Number of Employees
            string command;
            command = "select * from users";
            SqlDataAdapter adapter = new SqlDataAdapter(command, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            int empnum = 0;
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i][5].ToString() == "employee")
                {
                    empnum++;
                }
            }
            SqlCommand comm = new SqlCommand(command, conn);
            comm.ExecuteNonQuery();
            if (empnum * 250 <= this.Balance)
            {
                this.Balance -= empnum * 250;
                fillUserWith(this.Username);
                command = "select * from users";
                adapter = new SqlDataAdapter(command, conn);
                dataTable = new DataTable();
                adapter.Fill(dataTable);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (dataTable.Rows[i][5].ToString() == "employee")
                    {
                        string newcommand = "update books SET balance = '" + (int.Parse(dataTable.Rows[i][6].ToString()) + 250) + "' where id = '" + dataTable.Rows[i][0] + "'";
                        SqlCommand comm1 = new SqlCommand(command, conn);
                        comm1.ExecuteNonQuery();
                    }
                }
                comm = new SqlCommand(command, conn);
                comm.ExecuteNonQuery();
                conn.Close();
                return "Employee salaries were paid";
            }
            else
            {
                conn.Close();
                return "We don't have enough budget";
            }
        }
    }
    public class Employee : User, IShow, IChange
    {
        public Employee(string username, string emailAddress, string telephonenumber, string password, string type, int balance) : base(username, emailAddress, telephonenumber, password, type, balance)
        {

        }
        public Employee()
        {

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
    public class Member : User, IShow, IChange
    {
        public Member(string username, string emailAddress, string telephonenumber, string password, string type, int balance) : base(username, emailAddress, telephonenumber, password, type, balance)
        {

        }
        public Member()
        {

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
