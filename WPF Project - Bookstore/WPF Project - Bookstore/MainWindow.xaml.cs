﻿ousing System;
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
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace WPF_Project___Bookstore
{
    enum Types { Admin = 1, Employee, Member, NotFound, NotMatched, Bug }

    /// <summary>
    /// 1)Database Connection
    /// 2)Image Sources
    /// </summary>
    public partial class MainWindow : Window
    {
        //Temps
        string path;
        string temp_username, temp_email, temp_phone_number, temp_password;
        string temp_img_path = "";
        Types Access = Types.NotFound;
        Manager TempManager;
        Employee TempEmployee;
        Member TempMember;
        Book TempBook;
        int TempDeposit;
        string TempSearchBook;
        public List<string> RemoveEmployees = new List<string>();
        public List<int> BorrowedBooks = new List<int>();
        public List<int> ReturnedBooks = new List<int>();
        public ObservableCollection<Employee> Employees { get; set; } = new ObservableCollection<Employee>();
        public ObservableCollection<Member> Members { get; set; } = new ObservableCollection<Member>();
        public ObservableCollection<Book> Books { get; set; } = new ObservableCollection<Book>();



        public ImageSource Source { get; set; }



        //Main
        public MainWindow()
        {
            //Updates
            Member.Update_License_Time();
            Book.Update_Delay();
            //Initialize
            InitializeComponent();
            foreach (TabItem tab in Tabs.Items)
            {
                tab.Header = null;
                tab.Visibility = Visibility.Collapsed;
            }
        }









        //Database Functions
        private Types usernameAvailableAndMatch(string u, string p)
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
            if (name.Length != 16)
            {
                return false;
            }
            int[] digits = new int[16];
            int sum = 0;
            for (int i = 0; i < 16; i += 2)
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
        private bool MonthRegex(string year, string month)
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







        //Login page :
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

                            Books = TempBook.getAllBooks();
                            Bindi_Books.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

                            DataContext = this;
                            Manager_Balance.Text = "Balance : " + TempManager.Balance + " T";

                            path = Directory.GetCurrentDirectory();
                            path += @"\image\";
                            path += TempManager.Img_Path;
                            AvatarAdmin.ImageSource = new BitmapImage(new Uri(path));
                            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Main_Menu_Page));


                            break;
                        case Types.Employee:

                            Access = Types.Employee;
                            TempEmployee = new Employee();
                            TempBook = new Book();
                            if (TempEmployee.fillUserWith(LoginUsernameBox.Text) == false)
                            {
                                throw new Exception("Database Error!!!");
                            }
                            LoginUsernameBox.Text = "";
                            LoginPasswordBox.Password = "";


                            Members = TempEmployee.getUnreturned();
                            Member_Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

                            DataContext = this;


                            Employee_Balance.Text = "Balance : " + TempEmployee.Balance + " T";






                            EmployeeEditUsernameBox.Text = TempEmployee.Username;
                            EmployeeEditEmailBox.Text = TempEmployee.Email;
                            EmployeeEditPhoneNumberBox.Text = TempEmployee.Phone_Number;
                            EmployeeEditPasswordBox.Password = "";






                            path = Directory.GetCurrentDirectory();
                            path += @"\image\";
                            path += TempEmployee.Img_Path;
                            AvatarEmployee.ImageSource = new BitmapImage(new Uri(path));



                            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Employee_Main_Menu_Page));


                            break;
                        case Types.Member:
                            Access = Types.Member;
                            TempMember = new Member();
                            TempBook = new Book();
                            if (TempMember.fillUserWith(LoginUsernameBox.Text) == false)
                            {
                                throw new Exception("Database Error!!!");
                            }
                            LoginUsernameBox.Text = "";
                            LoginPasswordBox.Password = "";




                            MemberEditUsernameBox.Text = TempMember.Username;
                            MemberEditEmailBox.Text = TempMember.Email;
                            MemberEditPhoneNumberBox.Text = TempMember.Phone_Number;
                            MemberEditPasswordBox.Password = "";
                            Member_Balance.Text = "Balance : " + TempMember.Balance + " T";


                            if (TempMember.License_Time >= 0)
                            {
                                License_Border.Background = Brushes.GreenYellow.Clone();
                                Member_License.Text = "Your license is valid for " + TempMember.License_Time + " days";
                            }
                            else
                            {
                                License_Border.Background = Brushes.OrangeRed.Clone();
                                Member_License.Text = "Your license has expired for " + (-1) * TempMember.License_Time + " days";
                            }


                            path = Directory.GetCurrentDirectory();
                            path += @"\image\";
                            path += TempMember.Img_Path;
                            AvatarMember.ImageSource = new BitmapImage(new Uri(path));

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
            catch (Exception ee)
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
















        //Register page :
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
                    Register_PhoneNumber_Alert.Text = "Phone Number has already taken!";
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
            temp_img_path = "";
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
        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                temp_img_path = openFileDialog.FileName;
            }
        }














        //Payment page :
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
                    SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
                    conn.Open();
                    string command = "update users SET balance = '" + (TempManager.Balance + TempDeposit) + "' where username = '" + TempManager.Username + "'";
                    SqlCommand comm = new SqlCommand(command, conn);
                    comm.ExecuteNonQuery();
                    conn.Close();
                    TempManager.Balance += TempDeposit;
                    TempDeposit = 0;
                    Manager_Balance.Text = "Balance : " + TempManager.Balance + " T";
                    Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Bank_Account_Section_Page));
                }
                if (Access == Types.Member)
                {
                    SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
                    conn.Open();
                    string command = "update users SET balance = '" + (TempMember.Balance + TempDeposit) + "' where username = '" + TempMember.Username + "'";
                    SqlCommand comm = new SqlCommand(command, conn);
                    comm.ExecuteNonQuery();
                    conn.Close();
                    TempMember.Balance += TempDeposit;
                    TempDeposit = 0;
                    Member_Balance.Text = "Balance : " + TempMember.Balance + " T";
                    Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Bank_Account_Section_Page));
                }
                if (Access == Types.NotFound)
                {
                    string imagename = "";
                    if (temp_img_path == "")
                    {
                        imagename = "1.png";
                    }
                    else
                    {
                        imagename += temp_username.ToLower() + ".png";
                    }
                    TempMember = new Member(temp_username.ToLower(), temp_email.ToLower(), temp_phone_number, temp_password, "member", 0, DateTime.Now, imagename);
                    if (temp_img_path != "")
                    {
                        string destination_path = Directory.GetCurrentDirectory();
                        destination_path += @"\image\";
                        destination_path += imagename;
                        File.Copy(temp_img_path, destination_path, true);
                    }
                    path = Directory.GetCurrentDirectory();
                    path += @"\image\";
                    path += TempMember.Img_Path;
                    AvatarMember.ImageSource = new BitmapImage(new Uri(path));
                    if (TempMember.fillDatabase())
                    {
                        Access = Types.Member;
                        TempBook = new Book();


                        MemberEditUsernameBox.Text = TempMember.Username;
                        MemberEditEmailBox.Text = TempMember.Email;
                        MemberEditPhoneNumberBox.Text = TempMember.Phone_Number;
                        MemberEditPasswordBox.Password = "";
                        Member_Balance.Text = "Balance : " + TempMember.Balance + " T";


                        if (TempMember.License_Time >= 0)
                        {
                            License_Border.Background = Brushes.GreenYellow.Clone();
                            Member_License.Text = "Your license is valid for " + TempMember.License_Time + " days";
                        }
                        else
                        {
                            License_Border.Background = Brushes.OrangeRed.Clone();
                            Member_License.Text = "Your license has expired for " + (-1) * TempMember.License_Time + " days";
                        }

                        Books = TempBook.getAvailableBooks();
                        Bindi_Books_Member.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

                        DataContext = this;


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
            temp_img_path = "";
            Payment_CardNumber_Alert.Text = "";
            Payment_CVV_Alert.Text = "";
            Payment_Month_Alert.Text = "";
            CreditCardNumberBox.Text = "";
            CVV2Box.Text = "";
            monthBox.Text = "";
            YearBox.Text = "";
            if (Access == Types.Admin)
            {
                Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Bank_Account_Section_Page));
            }
            if (Access == Types.Member)
            {
                Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Bank_Account_Section_Page));
            }
            if (Access == Types.NotFound)
            {
                Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Register_Page));
            }
        }







        //Manager Main Menu Page :
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






        //Manager Employee Section :
        private void Manager_EmployeeSection_ListSection_AddButton_Click(object sender, RoutedEventArgs e)
        {
            Remove_All_Selected_Employee_Button.Visibility = Visibility.Hidden;
            Employees = TempManager.getEmployees();
            RemoveEmployees = new List<string>();
            Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            PayEmployeeButton.Visibility = Visibility.Visible;
            SubmitEmployeePay.Visibility = Visibility.Hidden;
            SubmitEmployeePayRect.Visibility = Visibility.Hidden;
            Alert_Employee_Pay.Text = "";
            PayEmployeePasswordBox.Password = "";
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Add_Employee_Menu_Page));
        }
        private void Manager_EmployeeSection_ListSection_PayButton_Click(object sender, RoutedEventArgs e)
        {
            PayEmployeeButton.Visibility = Visibility.Hidden;
            SubmitEmployeePay.Visibility = Visibility.Visible;
            SubmitEmployeePayRect.Visibility = Visibility.Visible;
        }
        private void Employees_Sumbmit_Pay_Click(object sender, RoutedEventArgs e)
        {
            if (TempManager.Password == PayEmployeePasswordBox.Password)
            {
                if (TempManager.payEmployee())
                {
                    Alert_Employee_Pay.Foreground = Brushes.Green;
                    Alert_Employee_Pay.Text = "Employee salaries were paid.";
                    PayEmployeeButton.Visibility = Visibility.Visible;
                    SubmitEmployeePay.Visibility = Visibility.Hidden;
                    SubmitEmployeePayRect.Visibility = Visibility.Hidden;
                    PayEmployeePasswordBox.Password = "";
                }
                else
                {
                    Alert_Employee_Pay.Foreground = Brushes.Red;
                    Alert_Employee_Pay.Text = "We don't have enough budget!";
                    PayEmployeeButton.Visibility = Visibility.Visible;
                    SubmitEmployeePay.Visibility = Visibility.Hidden;
                    SubmitEmployeePayRect.Visibility = Visibility.Hidden;
                    PayEmployeePasswordBox.Password = "";
                }
            }

            else
            {
                Alert_Employee_Pay.Foreground = Brushes.Red;
                Alert_Employee_Pay.Text = "Password is wrong!";
            }
        }
        private void Manager_EmployeeSection_ListSection_Back_Button_Click(object sender, RoutedEventArgs e)
        {
            Remove_All_Selected_Employee_Button.Visibility = Visibility.Hidden;
            Employees = TempManager.getEmployees();
            RemoveEmployees = new List<string>();
            Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            PayEmployeeButton.Visibility = Visibility.Visible;
            SubmitEmployeePay.Visibility = Visibility.Hidden;
            SubmitEmployeePayRect.Visibility = Visibility.Hidden;
            Alert_Employee_Pay.Text = "";
            PayEmployeePasswordBox.Password = "";
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













        //Manager Book Section :
        private void Manager_BooksSection_AddButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Add_Book_Menu_Page));
        }
        private void Manager_BooksSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Main_Menu_Page));
        }







        //manager bank acconut section :
        private void Manager_BankAccountSection_DepositButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TempDeposit = int.Parse(Manager_BankAccount_BankBalance_Deposit.Text);
                Manager_BankAccount_Alert.Text = "";
                Manager_BankAccount_BankBalance_Deposit.Text = "";
                Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Payment_Page));
            }
            catch (Exception)
            {
                Manager_BankAccount_Alert.Text = "Please enter the value in the correct format!";
            }
        }
        private void Manager_BankAccountSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Manager_BankAccount_Alert.Text = "";
            Manager_BankAccount_BankBalance_Deposit.Text = "";
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Main_Menu_Page));
        }







        //Manager Add Employee Section :
        private void AddEmployees_AddButton_Click(object sender, RoutedEventArgs e)
        {
            bool flag = true;
            if (UserNameRegex(AddEmployee_Username.Text))
            {
                if (usernameNotTaken(AddEmployee_Username.Text))
                {
                    AddEmployee_Usarname_Alert.Text = "";
                }
                else
                {
                    AddEmployee_Usarname_Alert.Text = "Username has already taken!";
                    flag = false;
                }
            }
            else
            {
                AddEmployee_Usarname_Alert.Text = "Username does not have the correct format!";
                flag = false;
            }
            if (EmailRegex(AddEmployeeEmailBox.Text))
            {
                if (emailNotTaken(AddEmployeeEmailBox.Text))
                {
                    AddEmployee_Email_Alert.Text = "";
                }
                else
                {
                    AddEmployee_Email_Alert.Text = "Email has already taken!";
                    flag = false;
                }
            }
            else
            {
                AddEmployee_Email_Alert.Text = "Email does not have the correct format!";
                flag = false;
            }
            if (PhoneNumberRegex(AddEmployeePhoneNumberBox.Text))
            {
                if (phoneNumberNotTaken(AddEmployeePhoneNumberBox.Text))
                {
                    AddEmployee_PhoneNumber_Alert.Text = "";
                }
                else
                {
                    AddEmployee_PhoneNumber_Alert.Text = "Password has already taken!";
                    flag = false;
                }
            }
            else
            {
                AddEmployee_PhoneNumber_Alert.Text = "Phone Number does not have the correct format!";
                flag = false;
            }
            if (PasswordRegex(AddEmployeePasswordBox.Password))
            {
                AddEmployee_Password_Alert.Text = "";
            }
            else
            {
                AddEmployee_Password_Alert.Foreground = Brushes.Red;
                AddEmployee_Password_Alert.Text = "Password does not have the correct format!";
                flag = false;
            }
            if (flag)
            {
                string imagename = "";
                if (temp_img_path == "")
                {
                    imagename = "1.png";
                }
                else
                {
                    imagename += AddEmployee_Username.Text.ToLower() + ".png";
                }
                TempEmployee = new Employee(AddEmployee_Username.Text.ToLower(), AddEmployeeEmailBox.Text.ToLower(), AddEmployeePhoneNumberBox.Text, AddEmployeePasswordBox.Password, "employee", 0, DateTime.Now, imagename);
                if (temp_img_path != "")
                {
                    string destination_path = Directory.GetCurrentDirectory();
                    destination_path += @"\image\";
                    destination_path += imagename;
                    File.Copy(temp_img_path, destination_path, true);
                }
                TempEmployee.fillDatabase();
                TempEmployee = null;
                Employees = TempManager.getEmployees();
                Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
                DataContext = this;
                AddEmployee_Username.Text = "";
                AddEmployeeEmailBox.Text = "";
                AddEmployeePhoneNumberBox.Text = "";
                AddEmployeePasswordBox.Password = "";
                AddEmployee_Password_Alert.Foreground = Brushes.Green;
                AddEmployee_Password_Alert.Text = "Employee Added.";
            }
        }
        private void AddEmployees_BackButton_Click(object sender, RoutedEventArgs e)
        {
            temp_img_path = "";
            AddEmployee_Usarname_Alert.Text = "";
            AddEmployee_Email_Alert.Text = "";
            AddEmployee_PhoneNumber_Alert.Text = "";
            AddEmployee_Password_Alert.Text = "";
            AddEmployee_Username.Text = "";
            AddEmployeeEmailBox.Text = "";
            AddEmployeePhoneNumberBox.Text = "";
            AddEmployeePasswordBox.Password = "";
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Manager_Employee_Section_List_Page));
        }
        private void btnOpenFile_Employee_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                temp_img_path = openFileDialog.FileName;
            }
        }






        //Manager Add Books Section :
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
                TempBook = new Book(-1, AddBooks_BookNameBox.Text, AddBooks_AuthorBox.Text, AddBooks_GenreBox.Text, AddBooks_PrintNoBox.Text, true, 0);
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
                Books = TempBook.getAllBooks();
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
            Books = TempBook.getBorrowedBooks();
            Bindi_Books_Employee.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
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
        private void Employee_All_Members(object sender, RoutedEventArgs e)
        {
            MembList.Text = "All";
            Members = TempEmployee.getAllMembers();
            Member_Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
        }
        private void Employee_Expired_License_Members(object sender, RoutedEventArgs e)
        {
            MembList.Text = "Expired License";
            Members = TempEmployee.getExpiredLicense();
            Member_Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
        }
        private void Employee_Unreturned_Members(object sender, RoutedEventArgs e)
        {
            MembList.Text = "Unreturned";
            Members = TempEmployee.getUnreturned();
            Member_Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
        }
        private void Employee_MembersSection_ListSection_SearchButton_Click(object sender, RoutedEventArgs e)
        {
            MemberUsernameSearch.Text = "";
            if (AllMembButton.Visibility == Visibility.Visible)
            {
                AllMembButton.Visibility = Visibility.Hidden;
                UnreturnedMembButton.Visibility = Visibility.Hidden;
                ExpiredLicenseMembButton.Visibility = Visibility.Hidden;
                MemberInfoRect.Visibility = Visibility.Visible;
                MemberInfo.Visibility = Visibility.Visible;
            }
            else
            {
                AllMembButton.Visibility = Visibility.Visible;
                UnreturnedMembButton.Visibility = Visibility.Visible;
                ExpiredLicenseMembButton.Visibility = Visibility.Visible;
                MemberInfoRect.Visibility = Visibility.Hidden;
                MemberInfo.Visibility = Visibility.Hidden;
            }
        }
        private void MemberInfoButton(object sender, RoutedEventArgs e)
        {
            Members = TempEmployee.getMemberwithUsername(MemberUsernameSearch.Text);
            if (Members.Count == 0)
            {
                MembList.Text = "Not Found!";
            }
            else
            {
                MembList.Text = MemberUsernameSearch.Text;
            }
            Member_Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
        }
        private void Employee_MembersSection_ListSection_Back_Button_Click(object sender, RoutedEventArgs e)
        {
            AllMembButton.Visibility = Visibility.Visible;
            UnreturnedMembButton.Visibility = Visibility.Visible;
            ExpiredLicenseMembButton.Visibility = Visibility.Visible;
            MemberInfoRect.Visibility = Visibility.Hidden;
            MemberInfo.Visibility = Visibility.Hidden;
            MembList.Text = "Unreturned";
            Members = TempEmployee.getUnreturned();
            Member_Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Employee_Main_Menu_Page));
        }
        private void Member_Information_Click(object sender, RoutedEventArgs e)
        {
            AllMembButton.Visibility = Visibility.Visible;
            UnreturnedMembButton.Visibility = Visibility.Visible;
            ExpiredLicenseMembButton.Visibility = Visibility.Visible;
            MemberInfoRect.Visibility = Visibility.Hidden;
            MemberInfo.Visibility = Visibility.Hidden;
            MembList.Text = "Unreturned";
            TempMember = new Member();
            TempMember.fillUserWith(((Button)sender).Tag.ToString());
            //Avatar
            path = Directory.GetCurrentDirectory();
            path += @"\image\";
            path += TempMember.Img_Path;
            AvatarMemberInEmployee.ImageSource = new BitmapImage(new Uri(path));
            //Fill Information Page
            MemberUsernameBox_Employee.Text = TempMember.Username;
            MemberEmailBox_Employee.Text = TempMember.Email;
            MemberPhoneNumberBox_Employee.Text = TempMember.Phone_Number;
            MemberRegistrationDateBox_Employee.Text = TempMember.Registration_Date.ToString();
            if (TempMember.Expired_License)
            {
                Information_License_Border.Background = Brushes.Red.Clone();
                Information_Member_License.Text = "License has expired for " + ((-1) * TempMember.License_Time) + " days";
            }
            else
            {
                Information_License_Border.Background = Brushes.GreenYellow.Clone();
                Information_Member_License.Text = "License is valid for " + TempMember.License_Time + " Days";
            }
            if (TempMember.Unreturned)
            {
                Information_Delay_Border.Background = Brushes.Red.Clone();
                Information_Member_Delay.Text = "Delayed";
            }
            else
            {
                Information_Delay_Border.Background = Brushes.GreenYellow.Clone();
                Information_Member_Delay.Text = "Without delay";
            }

            Books = TempMember.getBooksWithUsername();
            Member_Books_Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();


            Members = TempEmployee.getUnreturned();
            Member_Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Employee_Member_Information_Section_Page));
        }
        












        //Employee Book Section : 
        private void Employee_All_Books(object sender, RoutedEventArgs e)
        {
            qwer2.Text = "All Books";
            Books = TempBook.getAllBooks();
            Bindi_Books_Employee.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
        }
        private void Employee_Borrowed_Books(object sender, RoutedEventArgs e)
        {
            qwer2.Text = "Borrowed Books";
            Books = TempBook.getBorrowedBooks();
            Bindi_Books_Employee.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
        }
        private void Employee_Available_Books(object sender, RoutedEventArgs e)
        {
            qwer2.Text = "Available Books";
            Books = TempBook.getAvailableBooks();
            Bindi_Books_Employee.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
        }
        private void Employee_BooksSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            qwer2.Text = "Borrowed Books";
            Books = null;
            Bindi_Books_Employee.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Employee_Main_Menu_Page));
        }







        //Employee Wallet section : 
        private void Employee_BankAccountSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Employee_Main_Menu_Page));
        }









        //Employee Edit acconut section :
        private void Employee_Edit_Information_Button(object sender, RoutedEventArgs e)
        {
            bool flag = true;
            if (EmailRegex(EmployeeEditEmailBox.Text))
            {
                if (emailNotTaken(EmployeeEditEmailBox.Text) || EmployeeEditEmailBox.Text == TempEmployee.Email)
                {
                    EmployeeEdit_Email_Alert.Text = "";
                }
                else
                {
                    EmployeeEdit_Email_Alert.Text = "Email has already taken!";
                    flag = false;
                }
            }
            else
            {
                EmployeeEdit_Email_Alert.Text = "Email does not have the correct format!";
                flag = false;
            }
            if (PhoneNumberRegex(EmployeeEditPhoneNumberBox.Text))
            {
                if (phoneNumberNotTaken(EmployeeEditPhoneNumberBox.Text) || EmployeeEditPhoneNumberBox.Text == TempEmployee.Phone_Number)
                {
                    EmployeeEdit_PhoneNumber_Alert.Text = "";
                }
                else
                {
                    EmployeeEdit_PhoneNumber_Alert.Text = "Phone Number has already taken!";
                    flag = false;
                }
            }
            else
            {
                EmployeeEdit_PhoneNumber_Alert.Text = "Phone Number does not have the correct format!";
                flag = false;
            }
            if (usernameAvailableAndMatch(EmployeeEditUsernameBox.Text, EmployeeEditPasswordBox.Password) == Types.Employee)
            {
                EmployeeEdit_Password_Alert.Text = "";
            }
            else
            {
                EmployeeEdit_Password_Alert.Foreground = Brushes.Red;
                EmployeeEdit_Password_Alert.Text = "Password and Username aren't match";
                flag = false;
            }
            if (flag)
            {
                TempEmployee.editInformation(EmployeeEditEmailBox.Text, EmployeeEditPhoneNumberBox.Text);
                TempEmployee.Email = EmployeeEditEmailBox.Text;
                TempEmployee.Phone_Number = EmployeeEditPhoneNumberBox.Text;
                EmployeeEditPasswordBox.Password = "";
                EmployeeEdit_Password_Alert.Foreground = Brushes.Green;
                EmployeeEdit_Password_Alert.Text = "Employee's information changed.";
            }
        }
        private void Employee_EditAccountSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            EmployeeEditUsernameBox.Text = TempEmployee.Username;
            EmployeeEditEmailBox.Text = TempEmployee.Email;
            EmployeeEditPhoneNumberBox.Text = TempEmployee.Phone_Number;
            EmployeeEditPasswordBox.Password = "";
            EmployeeEdit_Password_Alert.Text = "";
            EmployeeEdit_PhoneNumber_Alert.Text = "";
            EmployeeEdit_Email_Alert.Text = "";
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Employee_Main_Menu_Page));
        }







        //Employee Member Information section :

        private void MemberInformation_Remove_Click(object sender, RoutedEventArgs e)
        {
            RemoveMemberButton_Name.Visibility = Visibility.Hidden;
            SubmitMemberRemoveRect.Visibility = Visibility.Visible;
            SubmitMemberRemove.Visibility = Visibility.Visible;
        }
        private void Remove_Member_Sumbmit_Click(object sender, RoutedEventArgs e)
        {
            if (TempEmployee.Password == RemoveMemberPasswordBox.Password)
            {
                if (TempMember != null)
                {
                    TempMember.removeThis();
                    TempMember = null;
                    RemoveMemberButton_Name.Visibility = Visibility.Visible;
                    SubmitMemberRemoveRect.Visibility = Visibility.Hidden;
                    SubmitMemberRemove.Visibility = Visibility.Hidden;
                    Alert_Remove_Member.Foreground = Brushes.Green;
                    Alert_Remove_Member.Text = "Member removed successfully!";
                    RemoveMemberPasswordBox.Password = "";
                    Books = null;
                    Member_Books_Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
                    DataContext = this;
                    MemberUsernameBox_Employee.Text = "";
                    MemberEmailBox_Employee.Text = "";
                    MemberPhoneNumberBox_Employee.Text = "";
                    MemberRegistrationDateBox_Employee.Text = "";
                    Information_License_Border.Background = Brushes.GreenYellow.Clone();
                    Information_Member_License.Text = "License is valid for " + 0 + " Days";
                    Information_Delay_Border.Background = Brushes.GreenYellow.Clone();
                    Information_Member_Delay.Text = "Without delay";
                }
                else
                {
                    Alert_Remove_Member.Foreground = Brushes.Red;
                    Alert_Remove_Member.Text = "The member has already been removed!";
                }
            }
            else
            {
                Alert_Remove_Member.Foreground = Brushes.Red;
                Alert_Remove_Member.Text = "Password is wrong!";
            }
        }
        private void Employee_MemberInformationSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveMemberButton_Name.Visibility = Visibility.Visible;
            SubmitMemberRemoveRect.Visibility = Visibility.Hidden;
            SubmitMemberRemove.Visibility = Visibility.Hidden;
            MemberUsernameBox_Employee.Text = "";
            MemberEmailBox_Employee.Text = "";
            MemberPhoneNumberBox_Employee.Text = "";
            MemberRegistrationDateBox_Employee.Text = "";
            Alert_Remove_Member.Text = "";
            RemoveMemberPasswordBox.Password = "";
            Information_License_Border.Background = Brushes.GreenYellow.Clone();
            Information_Member_License.Text = "License is valid for " + 0 + " Days";
            Information_Delay_Border.Background = Brushes.GreenYellow.Clone();
            Information_Member_Delay.Text = "Without delay";
            Books = null;
            Member_Books_Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
            AvatarMemberInEmployee.ImageSource = null;
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Employee_Member_Section_List_Page));
        }

















        //Member Main Menu Page :
        private void Member_BooksButton_Click(object sender, RoutedEventArgs e)
        {
            Books = TempBook.getAvailableBooks();
            Bindi_Books_Member.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Books_Section_List_Page));
        }
        private void Member_MyBookButton_Click(object sender, RoutedEventArgs e)
        {
            Books = TempMember.getBooksWithUsername();
            Bindi_MyBooks_Member.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
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
        private void Member_BooksSection_Search_Click(object sender, RoutedEventArgs e)
        {
            Borrow_Alert.Text = "";
            BorrowedBooks = new List<int>();
            Borrow_All_Selected_Books_Button.Visibility = Visibility.Hidden;
            Bindi_Books_Member.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
            qwer3.Foreground = Brushes.Gray;
            BookNameOrAuthorSearch.Text = "";
            if (BookSearchRect.Visibility == Visibility.Hidden)
            {
                qwer3.Text = "Book Search";
                Books = null;
                Bindi_Books_Member.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
                DataContext = this;
                BookSearchRect.Visibility = Visibility.Visible;
                BookSearch.Visibility = Visibility.Visible;
            }
            else
            {
                qwer3.Text = "Books";
                Books = TempBook.getAvailableBooks();
                Bindi_Books_Member.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
                DataContext = this;
                BookSearchRect.Visibility = Visibility.Hidden;
                BookSearch.Visibility = Visibility.Hidden;
            }
        }
        private void BookSearchButton(object sender, RoutedEventArgs e)
        {
            Borrow_Alert.Text = "";
            BorrowedBooks = new List<int>();
            Borrow_All_Selected_Books_Button.Visibility = Visibility.Hidden;
            Bindi_Books_Member.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
            TempSearchBook = BookNameOrAuthorSearch.Text;
            Books = TempBook.getSearchedBooks(TempSearchBook);
            if (Books.Count == 0)
            {
                qwer3.Foreground = Brushes.Red;
                qwer3.Text = "Not Found!";
            }
            else
            {
                qwer3.Foreground = Brushes.Gray;
                qwer3.Text = "Book Search";
            }
            Bindi_Books_Member.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
        }
        private void Member_BooksSection_ListSection_Back_Button_Click(object sender, RoutedEventArgs e)
        {
            qwer3.Text = "Books";
            qwer3.Foreground = Brushes.Gray;
            Books = null;
            Borrow_Alert.Text = "";
            BorrowedBooks = new List<int>();
            Borrow_All_Selected_Books_Button.Visibility = Visibility.Hidden;
            Bindi_Books_Member.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
            BookSearchRect.Visibility = Visibility.Hidden;
            BookSearch.Visibility = Visibility.Hidden;
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Main_Menu_Page));
        }
        private void Borrow_Book_Click(object sender, RoutedEventArgs e)
        {
            if (((Button)sender).Width == 10)
            {
                if (BorrowedBooks.Count + TempMember.Borrowed_Number < 5)
                {
                    ((Button)sender).Width = 10.5;
                    ((Button)sender).Height = 10.5;
                    ((Button)sender).Background = Brushes.DeepPink.Clone();
                    BorrowedBooks.Add((int)((Button)sender).Tag);
                    Borrow_All_Selected_Books_Button.Visibility = Visibility.Visible;
                }
                else
                {
                    Borrow_Alert.Text = "You can't borrow more than 5 books!";
                }
            }
            else
            {
                Borrow_Alert.Text = "";
                ((Button)sender).Width = 10;
                ((Button)sender).Height = 10;
                ((Button)sender).Background = Brushes.YellowGreen.Clone();
                BorrowedBooks.Remove((int)((Button)sender).Tag);
                if (BorrowedBooks.Count == 0)
                {
                    Borrow_All_Selected_Books_Button.Visibility = Visibility.Hidden;
                }
            }
        }
        private void Borrow_Books_Click2(object sender, RoutedEventArgs e)
        {
            if (TempMember.License_Time < 7)
            {
                Borrow_Alert.Foreground = Brushes.Red;
                Borrow_Alert.Text = "Your license has expired or is about to expire!";
                return;
            }
            if (TempMember.Unreturned)
            {
                Borrow_Alert.Foreground = Brushes.Red;
                Borrow_Alert.Text = "You have a delayed book, please return it first!";
                return;
            }

            Borrow_Alert.Text = "";
            TempMember.editBorrowedNumber(TempMember.Borrowed_Number + BorrowedBooks.Count);
            TempBook.borrowSelectedBooks(BorrowedBooks, TempMember.Username);
            BorrowedBooks = new List<int>();
            if (BookSearchRect.Visibility == Visibility.Visible)
            {
                Books = TempBook.getSearchedBooks(TempSearchBook);
            }
            else
            {
                Books = TempBook.getAvailableBooks();
            }
            Borrow_All_Selected_Books_Button.Visibility = Visibility.Hidden;
            Bindi_Books_Member.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
        }











        //Member MyBooks Section :
        private void Return_Book_Click(object sender, RoutedEventArgs e)
        {
            Return_Alert.Text = "";
            if (((Button)sender).Width == 10)
            {
                if (true)
                {
                    ((Button)sender).Width = 10.5;
                    ((Button)sender).Height = 10.5;
                    ((Button)sender).Background = Brushes.DeepPink.Clone();
                    ReturnedBooks.Add((int)((Button)sender).Tag);
                    Return_All_Selected_Books_Button.Visibility = Visibility.Visible;
                }
            }
            else
            {
                ((Button)sender).Width = 10;
                ((Button)sender).Height = 10;
                ((Button)sender).Background = Brushes.YellowGreen.Clone();
                ReturnedBooks.Remove((int)((Button)sender).Tag);
                if (ReturnedBooks.Count == 0)
                {
                    Return_All_Selected_Books_Button.Visibility = Visibility.Hidden;
                }
            }
        }
        private void Return_Books_Click2(object sender, RoutedEventArgs e)
        {
            int sum = 0;
            for (int i = 0; i < ReturnedBooks.Count; i++)
            {
                for (int j = 0; j < Books.Count; j++)
                {
                    if (ReturnedBooks[i] == Books[j].ID)
                    {
                        if (Books[j].Delay > 0)
                        {
                            sum += Books[j].Delay;
                        }
                        break;
                    }
                }
            }
            if (sum <= TempMember.Balance)
            {

                Return_Alert.Foreground = Brushes.Green;
                Return_Alert.Text = "The books were returned and you were fined " + sum + " T";
                TempBook.returnSelectedBooks(ReturnedBooks, TempMember.Username);
                TempMember.editBorrowedNumber(TempMember.Borrowed_Number - ReturnedBooks.Count);
                TempMember.returnBooks(sum, ReturnedBooks);
                ReturnedBooks = new List<int>();
                Books = TempMember.getBooksWithUsername();
                Member_Balance.Text = "Balance : " + TempMember.Balance + " T";
                Return_All_Selected_Books_Button.Visibility = Visibility.Hidden;
                Bindi_MyBooks_Member.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
                DataContext = this;
            }
            else
            {
                Return_Alert.Foreground = Brushes.Red;
                Return_Alert.Text = "You have " + (sum - TempMember.Balance) + " T less to pay your fine";
            }
        }
        private void Member_BooksSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Books = null;
            Bindi_MyBooks_Member.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            DataContext = this;
            Return_Alert.Text = "";
            ReturnedBooks = new List<int>();
            Return_All_Selected_Books_Button.Visibility = Visibility.Hidden;
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Main_Menu_Page));
        }







        //Member License section :
        private void Member_License_Renew_Click(object sender, RoutedEventArgs e)
        {
            if (TempMember.Balance >= 100)
            {
                Member_License_Renew_Alert.Foreground = Brushes.Green;
                TempMember.renewLicense();
                if (TempMember.License_Time >= 0)
                {
                    License_Border.Background = Brushes.GreenYellow.Clone();
                    Member_License.Text = "Your license is valid for " + TempMember.License_Time + " days";
                }
                else
                {
                    License_Border.Background = Brushes.OrangeRed.Clone();
                    Member_License.Text = "Your license has expired for " + (-1) * TempMember.License_Time + " days";
                }
                Member_Balance.Text = "Balance : " + TempMember.Balance + " T";
                Member_License_Renew_Alert.Text = "Your license has been successfully renewed.";
            }
            else
            {
                Member_License_Renew_Alert.Foreground = Brushes.Red;
                Member_License_Renew_Alert.Text = "You have " + (100 - TempMember.Balance) + " T less to renew your license!";
            }
        }
        private void Member_LicenseSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Member_License_Renew_Alert.Text = "";
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Main_Menu_Page));
        }










        //Member Bank Account section : 
        private void Member_BankAccountSection_DepositButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                TempDeposit = int.Parse(Member_Wallet_BankBalance_Deposit.Text);
                Member_Wallet_Alert.Text = "";
                Member_Wallet_BankBalance_Deposit.Text = "";
                Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Payment_Page));
            }
            catch (Exception)
            {
                Member_Wallet_Alert.Text = "Please enter the value in the correct format!";
            }
        }
        private void Member_Bank_AccountSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            Member_Wallet_Alert.Text = "";
            Member_Wallet_BankBalance_Deposit.Text = "";
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Main_Menu_Page));
        }








        //Member Edit information section : 
        private void Member_Edit_Information_Button(object sender, RoutedEventArgs e)
        {
            bool flag = true;
            if (EmailRegex(MemberEditEmailBox.Text))
            {
                if (emailNotTaken(MemberEditEmailBox.Text) || MemberEditEmailBox.Text == TempMember.Email)
                {
                    MemberEdit_Email_Alert.Text = "";
                }
                else
                {
                    MemberEdit_Email_Alert.Text = "Email has already taken!";
                    flag = false;
                }
            }
            else
            {
                MemberEdit_Email_Alert.Text = "Email does not have the correct format!";
                flag = false;
            }
            if (PhoneNumberRegex(MemberEditPhoneNumberBox.Text))
            {
                if (phoneNumberNotTaken(MemberEditPhoneNumberBox.Text) || MemberEditPhoneNumberBox.Text == TempMember.Phone_Number)
                {
                    MemberEdit_PhoneNumber_Alert.Text = "";
                }
                else
                {
                    MemberEdit_PhoneNumber_Alert.Text = "Phone Number has already taken!";
                    flag = false;
                }
            }
            else
            {
                MemberEdit_PhoneNumber_Alert.Text = "Phone Number does not have the correct format!";
                flag = false;
            }
            if (usernameAvailableAndMatch(MemberEditUsernameBox.Text, MemberEditPasswordBox.Password) == Types.Member)
            {
                MemberEdit_Password_Alert.Text = "";
            }
            else
            {
                MemberEdit_Password_Alert.Foreground = Brushes.Red;
                MemberEdit_Password_Alert.Text = "Password and Username aren't match";
                flag = false;
            }
            if (flag)
            {
                TempMember.editInformation(MemberEditEmailBox.Text, MemberEditPhoneNumberBox.Text);
                TempMember.Email = MemberEditEmailBox.Text;
                TempMember.Phone_Number = MemberEditPhoneNumberBox.Text;
                MemberEditPasswordBox.Password = "";
                MemberEdit_Password_Alert.Foreground = Brushes.Green;
                MemberEdit_Password_Alert.Text = "Member's information changed.";
            }
        }
        private void Member_EditAccountSection_BackButton_Click(object sender, RoutedEventArgs e)
        {
            MemberEditUsernameBox.Text = TempMember.Username;
            MemberEditEmailBox.Text = TempMember.Email;
            MemberEditPhoneNumberBox.Text = TempMember.Phone_Number;
            MemberEditPasswordBox.Password = "";
            MemberEdit_Password_Alert.Text = "";
            MemberEdit_PhoneNumber_Alert.Text = "";
            MemberEdit_Email_Alert.Text = "";
            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Member_Main_Menu_Page));
        }





        //Exit
        private void Exit_Button_Click(object sender, RoutedEventArgs e)
        {
            //Reset Temps
            Employees = new ObservableCollection<Employee>();
            Members = new ObservableCollection<Member>();
            Books = new ObservableCollection<Book>();
            Access = Types.NotFound;
            temp_username = temp_email = temp_phone_number = temp_password = "";
            TempManager = null;
            TempEmployee = null;
            TempMember = null;
            TempBook = null;
            TempDeposit = 0;
            TempSearchBook = "";
            temp_img_path = "";
            List<string> RemoveEmployees = new List<string>();
            List<int> BorrowedBooks = new List<int>();
            List<int> ReturnedBooks = new List<int>();

            //Empty Tables
            Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

            Bindi_Books.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

            Member_Bindi.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

            Bindi_Books_Employee.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

            Bindi_Books_Member.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();

            DataContext = this;

            Manager_Balance.Text = "Balance : " + 0 + " T";
            Employee_Balance.Text = "Balance : " + 0 + " T";
            Member_Balance.Text = "Balance : " + 0 + " T";

            License_Border.Background = Brushes.GreenYellow.Clone();
            Member_License.Text = "Your license is valid for " + 0 + " days";


            EmployeeEditUsernameBox.Text = "";
            EmployeeEditEmailBox.Text = "";
            EmployeeEditPhoneNumberBox.Text = "";
            EmployeeEditPasswordBox.Password = "";

            MemberEditUsernameBox.Text = "";
            MemberEditEmailBox.Text = "";
            MemberEditPhoneNumberBox.Text = "";
            MemberEditPasswordBox.Password = "";


            AvatarAdmin.ImageSource = null;
            AvatarEmployee.ImageSource = null;
            AvatarMember.ImageSource = null;

            Dispatcher.BeginInvoke((Action)(() => Tabs.SelectedItem = Login_Page));
        }


        //Exit & Minimize
        private void Exit_App(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Minimize_App(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }
    }











    //Classes
    public class Book
    {
        int _id;
        string _name;
        string _author;
        string _genre;
        string _publish_no;
        bool _availability;
        string _borrowed_username;
        DateTime _borrow_date;
        int _delay;
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
        public int ID
        {
            get { return this._id; }
            set { this._id = value; }
        }
        public bool Availability
        {
            get { return this._availability; }
            set { this._availability = value; }
        }
        public string Borrowed_Username
        {
            get { return this._borrowed_username; }
            set { this._borrowed_username = value; }
        }
        public DateTime Borrow_Date
        {
            get { return this._borrow_date; }
            set { this._borrow_date = value; }
        }
        public int Delay
        {
            get { return this._delay; }
            set { this._delay = value; }
        }
        public static void Update_Delay()
        {
            SqlConnection conn1 = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn1.Open();
            string command = "select * from books";
            SqlDataAdapter adapter = new SqlDataAdapter(command, conn1);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                DateTime BorTime = (DateTime)dataTable.Rows[i][7];
                if ((bool)dataTable.Rows[i][5] == false)
                {
                    int del = (-1)*((BorTime.Year - (DateTime.Now.Year)) * 360 + (BorTime.Month - DateTime.Now.Month) * 30 + (BorTime.Day - DateTime.Now.Day)) - 7;
                    string newcommand;
                    newcommand = "update books SET delay = '" + (del) + "' where id = '" + dataTable.Rows[i][0] + "'";
                    SqlCommand comm2 = new SqlCommand(newcommand, conn1);
                    comm2.ExecuteNonQuery();
                    if (del > 0)
                    {
                        newcommand = "update users SET unreturned = '" + true + "' where username = '" + dataTable.Rows[i][6] + "'";
                        comm2 = new SqlCommand(newcommand, conn1);
                        comm2.ExecuteNonQuery();
                    }
                }
            }
            SqlCommand comm1 = new SqlCommand(command, conn1);
            comm1.ExecuteNonQuery();
            conn1.Close();
        }
        public Book(int id, string name, string author, string genre, string printno,bool availability,int deley)
        {
            this._name = name;
            this._author = author;
            this._genre = genre;
            this._publish_no = printno;
            this._id = id;
            this._availability = availability;
            this._borrowed_username = "-1";
            this._borrow_date = DateTime.Now;
            this._delay = deley;
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
                //Find Last ID
                int last_id = 1;
                string command = "select id from books";
                SqlDataAdapter adapter = new SqlDataAdapter(command, conn1);
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                if (dataTable.Rows.Count > 0)
                {
                    last_id = int.Parse(dataTable.Rows[dataTable.Rows.Count - 1][0].ToString()) + 1;
                }
                SqlCommand comm1 = new SqlCommand(command, conn1);
                comm1.ExecuteNonQuery();
                conn1.Close();
                //Insert Data
                conn1.Open();
                command = "insert into books values('" + last_id + "','" + this._name + "','" + this._author + "','" + this._genre + "','" + this._publish_no + "', '" + true + "', '" + (-1) + "' , '" + DateTime.Now + "', '" + 0 + "' )";
                comm1 = new SqlCommand(command, conn1);
                comm1.ExecuteNonQuery();
                conn1.Close();
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public ObservableCollection<Book> getAllBooks()
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
                Bks.Add(new Book(int.Parse(dataTable.Rows[i][0].ToString()),
                                           dataTable.Rows[i][1].ToString(),
                                           dataTable.Rows[i][2].ToString(),
                                           dataTable.Rows[i][3].ToString(),
                                           dataTable.Rows[i][4].ToString(),
                                           (bool)dataTable.Rows[i][5],
                                           (int)dataTable.Rows[i][8]));
            }
            SqlCommand comm = new SqlCommand(command, conn);
            comm.ExecuteNonQuery();
            conn.Close();
            return Bks;
        }
        public ObservableCollection<Book> getBorrowedBooks()
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
                if ((bool)dataTable.Rows[i][5] == false)
                {
                    Bks.Add(new Book(int.Parse(dataTable.Rows[i][0].ToString()),
                                               dataTable.Rows[i][1].ToString(),
                                               dataTable.Rows[i][2].ToString(),
                                               dataTable.Rows[i][3].ToString(),
                                               dataTable.Rows[i][4].ToString(),
                                               (bool)dataTable.Rows[i][5],
                                               (int)dataTable.Rows[i][8]));
                }
            }
            SqlCommand comm = new SqlCommand(command, conn);
            comm.ExecuteNonQuery();
            conn.Close();
            return Bks;
        }
        public ObservableCollection<Book> getAvailableBooks()
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
                if ((bool)dataTable.Rows[i][5] == true)
                {
                    Bks.Add(new Book(int.Parse(dataTable.Rows[i][0].ToString()),
                                           dataTable.Rows[i][1].ToString(),
                                           dataTable.Rows[i][2].ToString(),
                                           dataTable.Rows[i][3].ToString(),
                                           dataTable.Rows[i][4].ToString(),
                                           (bool)dataTable.Rows[i][5],
                                           (int)dataTable.Rows[i][8]));
                }
            }
            SqlCommand comm = new SqlCommand(command, conn);
            comm.ExecuteNonQuery();
            conn.Close();
            return Bks;
        }
        public ObservableCollection<Book> getSearchedBooks(string serach)
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
                if ((bool)dataTable.Rows[i][5] == true && (dataTable.Rows[i][1].ToString().ToLower() == serach.ToLower() || dataTable.Rows[i][2].ToString().ToLower() == serach.ToLower()))
                {
                    Bks.Add(new Book(int.Parse(dataTable.Rows[i][0].ToString()),
                                           dataTable.Rows[i][1].ToString(),
                                           dataTable.Rows[i][2].ToString(),
                                           dataTable.Rows[i][3].ToString(),
                                           dataTable.Rows[i][4].ToString(),
                                           (bool)dataTable.Rows[i][5],
                                           (int)dataTable.Rows[i][8]));
                }
            }
            SqlCommand comm = new SqlCommand(command, conn);
            comm.ExecuteNonQuery();
            conn.Close();
            return Bks;
        }
        public void borrowSelectedBooks(List<int> books , string username)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            for (int i = 0; i < books.Count; i++)
            {
                this._borrowed_username = username;
                this._availability = false;
                this._borrow_date = DateTime.Now;
                this._delay = -7;
                command = "update books SET borrowed_username = '" + username + "', availability = '" + false + "', borrow_date = '" + DateTime.Now + "', delay = '" + -7 + "' where id = '" + books[i] + "'";
                SqlCommand comm = new SqlCommand(command, conn);
                comm.ExecuteNonQuery();
            }
            conn.Close();
        }
        public void returnSelectedBooks(List<int> books, string username)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            for (int i = 0; i < books.Count; i++)
            {
                this._borrowed_username = "-1";
                this._availability = true;
                this._borrow_date = DateTime.Now;
                this._delay = 0;
                command = "update books SET borrowed_username = '" + "-1" + "', availability = '" + true + "', borrow_date = '" + DateTime.Now + "', delay = '" + 0 + "' where id = '" + books[i] + "'";
                SqlCommand comm = new SqlCommand(command, conn);
                comm.ExecuteNonQuery();
            }
            conn.Close();
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
        private DateTime registration_date;
        private string img_path;
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
        public DateTime Registration_Date
        {
            get { return this.registration_date; }
            set { this.registration_date = value; }
        }
        public string Img_Path
        {
            get { return this.img_path; }
            set { this.img_path = value; }
        }
        public User(string username, string emailAddress, string telephonenumber, string password, string type , int balance , DateTime registration_date, string imgpath)
        {
            this.username = username;
            this.email = emailAddress;
            this.phone_number = telephonenumber;
            this.password = password;
            this.type = type;
            this.balance = balance;
            this.registration_date = registration_date;
            this.img_path = imgpath;
        }
        public User()
        {

        }
        public virtual bool fillDatabase()
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
                comm1.ExecuteNonQuery();
                conn1.Close();

                //Insert Data
                SqlConnection conn2 = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
                conn2.Open();
                command = "insert into users values('" + last_id + "','" + this.username.Trim() + "','" + this.email.Trim() + "','" + this.phone_number.Trim() + "','" + this.password.Trim() + "','" + type.Trim() + "','" + this.balance + "','" + null + "','" + null + "','" + this.registration_date + "', '" + null + "' , '" + null + "', '" + null + "','" + this.Img_Path + "')";
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
        public virtual bool fillUserWith(string username)
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
                        this.registration_date = (DateTime)dataTable.Rows[i][9];
                        this.img_path = dataTable.Rows[i][13].ToString();
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

        public Manager(string username, string emailAddress, string telephonenumber, string password, string type, int balance, DateTime registration_date, string imgpath) : base(username, emailAddress, telephonenumber, password, type, balance, registration_date, imgpath)
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
                                          int.Parse(dataTable.Rows[i][6].ToString()),
                                          (DateTime)dataTable.Rows[i][9],
                                          dataTable.Rows[i][13].ToString()));
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
        public bool payEmployee()
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
                //Update Admin Balance
                this.Balance -= empnum * 250;
                command = "update users SET balance = '" + (this.Balance) + "' where username = '" + this.Username + "'";
                SqlCommand comm1 = new SqlCommand(command, conn);
                comm1.ExecuteNonQuery();


                //Update Employees Balance
                command = "select * from users";
                adapter = new SqlDataAdapter(command, conn);
                dataTable = new DataTable();
                adapter.Fill(dataTable);
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    if (dataTable.Rows[i][5].ToString() == "employee")
                    {
                        string newcommand = "update users SET balance = '" + (int.Parse(dataTable.Rows[i][6].ToString()) + 250) + "' where id = '" + dataTable.Rows[i][0] + "'";
                        comm1 = new SqlCommand(newcommand, conn);
                        comm1.ExecuteNonQuery();
                    }
                }
                comm = new SqlCommand(command, conn);
                comm.ExecuteNonQuery();
                conn.Close();
                return true;
            }
            else
            {
                conn.Close();
                return false;
            }
        }
    }
    public class Employee : User
    {
        public Employee(string username, string emailAddress, string telephonenumber, string password, string type, int balance, DateTime registration_date, string imgpath) : base(username, emailAddress, telephonenumber, password, type, balance, registration_date, imgpath)
        {

        }
        public Employee()
        {

        }
        public ObservableCollection<Member> getAllMembers()
        {
            ObservableCollection<Member> Membs = new ObservableCollection<Member>();
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            command = "select * from users";
            SqlDataAdapter adapter = new SqlDataAdapter(command, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i][5].ToString() == "member")
                {
                    Membs.Add(new Member(dataTable.Rows[i][1].ToString(),
                                          dataTable.Rows[i][2].ToString(),
                                          dataTable.Rows[i][3].ToString(),
                                          dataTable.Rows[i][4].ToString(),
                                          dataTable.Rows[i][5].ToString(),
                                          int.Parse(dataTable.Rows[i][6].ToString()),
                                          (DateTime)dataTable.Rows[i][9],
                                          dataTable.Rows[i][13].ToString()));
                }
            }
            SqlCommand comm = new SqlCommand(command, conn);
            comm.BeginExecuteNonQuery();
            conn.Close();
            return Membs;
        }
        public ObservableCollection<Member> getUnreturned()
        {
            ObservableCollection<Member> Membs = new ObservableCollection<Member>();
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            command = "select * from users";
            SqlDataAdapter adapter = new SqlDataAdapter(command, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i][5].ToString() == "member")
                {
                    if(dataTable.Rows[i][7].ToString() == "True")
                    Membs.Add(new Member(dataTable.Rows[i][1].ToString(),
                                          dataTable.Rows[i][2].ToString(),
                                          dataTable.Rows[i][3].ToString(),
                                          dataTable.Rows[i][4].ToString(),
                                          dataTable.Rows[i][5].ToString(),
                                          int.Parse(dataTable.Rows[i][6].ToString()),
                                          (DateTime)dataTable.Rows[i][9],
                                          dataTable.Rows[i][13].ToString()));
                }
            }
            SqlCommand comm = new SqlCommand(command, conn);
            comm.BeginExecuteNonQuery();
            conn.Close();
            return Membs;
        }
        public ObservableCollection<Member> getExpiredLicense()
        {
            ObservableCollection<Member> Membs = new ObservableCollection<Member>();
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            command = "select * from users";
            SqlDataAdapter adapter = new SqlDataAdapter(command, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i][5].ToString() == "member")
                {
                    if (dataTable.Rows[i][8].ToString() == "True")
                        Membs.Add(new Member(dataTable.Rows[i][1].ToString(),
                                              dataTable.Rows[i][2].ToString(),
                                              dataTable.Rows[i][3].ToString(),
                                              dataTable.Rows[i][4].ToString(),
                                              dataTable.Rows[i][5].ToString(),
                                              int.Parse(dataTable.Rows[i][6].ToString()),
                                              (DateTime)dataTable.Rows[i][9],
                                              dataTable.Rows[i][13].ToString()));
                }
            }
            SqlCommand comm = new SqlCommand(command, conn);
            comm.BeginExecuteNonQuery();
            conn.Close();
            return Membs;
        }
        public ObservableCollection<Member> getMemberwithUsername(string u)
        {
            ObservableCollection<Member> Membs = new ObservableCollection<Member>();
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            command = "select * from users";
            SqlDataAdapter adapter = new SqlDataAdapter(command, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i][5].ToString() == "member")
                {
                    if (dataTable.Rows[i][1].ToString() == u)
                        Membs.Add(new Member(dataTable.Rows[i][1].ToString(),
                                              dataTable.Rows[i][2].ToString(),
                                              dataTable.Rows[i][3].ToString(),
                                              dataTable.Rows[i][4].ToString(),
                                              dataTable.Rows[i][5].ToString(),
                                              int.Parse(dataTable.Rows[i][6].ToString()),
                                              (DateTime)dataTable.Rows[i][9],
                                              dataTable.Rows[i][13].ToString()));
                }
            }
            SqlCommand comm = new SqlCommand(command, conn);
            comm.BeginExecuteNonQuery();
            conn.Close();
            return Membs;
        }
        public void editInformation(string newemail, string newphone)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            command = "update users SET email = '" + newemail.Trim() + "' , phone_number = '" + newphone.Trim() + "' where username = '" + this.Username + "'";
            SqlCommand comm = new SqlCommand(command, conn);
            comm.ExecuteNonQuery();
            conn.Close();
        }
    }
    public class Member : User
    {
        private bool unreturned;
        private bool expired_license;
        private int license_time;
        private int license_number;
        private int borrow_number;
        public bool Unreturned
        {
            get { return this.unreturned; }
            set { this.unreturned = value; }
        }
        public bool Expired_License
        {
            get { return this.expired_license; }
            set { this.expired_license = value; }
        }
        public int License_Time
        {
            get { return this.license_time; }
            set { this.license_time = value; }
        }
        public int License_Number
        {
            get { return this.license_number; }
            set { this.license_number = value; }
        }
        public int Borrowed_Number
        {
            get { return this.borrow_number; }
            set { this.borrow_number = value; }
        }
        public static void Update_License_Time()
        {
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            //Update Members License Time
            command = "select * from users";
            SqlDataAdapter adapter = new SqlDataAdapter(command, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i][5].ToString() == "member")
                {
                    DateTime Regtime = (DateTime)dataTable.Rows[i][9];
                    int lict = (Regtime.Year - (DateTime.Now.Year)) * 360 + (Regtime.Month - DateTime.Now.Month) * 30 + (Regtime.Day - DateTime.Now.Day);
                    string newcommand;
                    newcommand = "update users SET license_time = '" + (lict + 30 * int.Parse(dataTable.Rows[i][11].ToString())) + "', expired_license = '" + (lict + 30 * int.Parse(dataTable.Rows[i][11].ToString()) < 0) + "'  where id = '" + dataTable.Rows[i][0] + "'";
                    SqlCommand comm1 = new SqlCommand(newcommand, conn);
                    comm1.ExecuteNonQuery();
                }
            }
            SqlCommand comm = new SqlCommand(command, conn);
            comm.ExecuteNonQuery();
            conn.Close();
        }
        public Member(string username, string emailAddress, string telephonenumber, string password, string type, int balance, DateTime registration_date, string imgpath) : base(username, emailAddress, telephonenumber, password, type, balance, registration_date, imgpath)
        {
            this.unreturned = false;
            this.expired_license = false;
            this.license_time = 30;
            this.license_number = 1;
            this.borrow_number = 0;
        }
        public Member()
        {

        }
        public void editInformation(string newemail, string newphone)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            command = "update users SET email = '" + newemail.Trim() + "' , phone_number = '" + newphone.Trim() + "' where username = '" + this.Username + "'";
            SqlCommand comm = new SqlCommand(command, conn);
            comm.ExecuteNonQuery();
            conn.Close();
        }
        public void renewLicense()
        {
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            this.Balance -= 100;
            this.License_Time += 30;
            this.License_Number += 1;
            if (this.License_Time + 30 < 0)
            {
                this.Expired_License = true;
            }
            else
            {
                this.Expired_License = false;
            }
            command = "update users SET balance = '" + this.Balance + "' , license_time = '" + this.License_Time + "', license_number = '" + this.License_Number + "', expired_license = '" + this.Expired_License + "' where username = '" + this.Username + "'";
            SqlCommand comm = new SqlCommand(command, conn);
            comm.ExecuteNonQuery();
            conn.Close();
        }
        public override bool fillDatabase()
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
                comm1.ExecuteNonQuery();
                conn1.Close();

                //Insert Data
                SqlConnection conn2 = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
                conn2.Open();
                command = "insert into users values('" + last_id + "','" + this.Username.Trim() + "','" + this.Email.Trim() + "','" + this.Phone_Number.Trim() + "','" + this.Password.Trim() + "','" + this.Type.Trim() + "','" + this.Balance + "','" + false + "','" + false + "','" + this.Registration_Date + "','" + 30 + "','" + 1 + "', '" + 0 + "','" + this.Img_Path.ToString() + "')";
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
        public override bool fillUserWith(string username)
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
                        this.Username = dataTable.Rows[i][1].ToString();
                        this.Email = dataTable.Rows[i][2].ToString();
                        this.Phone_Number = dataTable.Rows[i][3].ToString();
                        this.Password = dataTable.Rows[i][4].ToString();
                        this.Type = dataTable.Rows[i][5].ToString();
                        this.Balance = int.Parse(dataTable.Rows[i][6].ToString());
                        this.Unreturned = (bool)dataTable.Rows[i][7];
                        this.Expired_License = (bool)dataTable.Rows[i][8];
                        this.Registration_Date = (DateTime)dataTable.Rows[i][9];
                        this.License_Time = int.Parse(dataTable.Rows[i][10].ToString());
                        this.License_Number = int.Parse(dataTable.Rows[i][11].ToString());
                        this.borrow_number = int.Parse(dataTable.Rows[i][12].ToString());
                        this.Img_Path = dataTable.Rows[i][13].ToString();
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

        public void editBorrowedNumber(int bn)
        {
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            this.borrow_number = bn;
            command = "update users SET borrowed_number = '" + bn + "' where username = '" + this.Username + "'";
            SqlCommand comm = new SqlCommand(command, conn);
            comm.ExecuteNonQuery();
            conn.Close();
        }
        public ObservableCollection<Book> getBooksWithUsername()
        {
            ObservableCollection<Book> BorBooks = new ObservableCollection<Book>();
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            command = "select * from books";
            SqlDataAdapter adapter = new SqlDataAdapter(command, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i][6].ToString() == this.Username)
                {
                    BorBooks.Add(new Book(int.Parse(dataTable.Rows[i][0].ToString()),
                                          dataTable.Rows[i][1].ToString(),
                                          dataTable.Rows[i][2].ToString(),
                                          dataTable.Rows[i][3].ToString(),
                                          dataTable.Rows[i][4].ToString(),
                                          (bool)dataTable.Rows[i][5],
                                          (int)dataTable.Rows[i][8]));
                }
            }
            return BorBooks;
        }
        public void returnBooks(int fine , List<int> returnedbooks)
        {
            this.Balance -= fine;
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command;
            command = "select * from books";
            SqlDataAdapter adapter = new SqlDataAdapter(command, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            bool flagunreturned = false;
            for(int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i][6].ToString() == this.Username && (int)dataTable.Rows[i][8] > 0)
                {
                    flagunreturned = true;
                }
            }
            SqlCommand comm = new SqlCommand(command, conn);
            comm.ExecuteNonQuery();
            this.Unreturned = flagunreturned;
            command = "update users SET balance = '" + this.Balance + "', unreturned = '" + this.Unreturned + "'  where username = '" + this.Username + "'";
            comm = new SqlCommand(command, conn);
            comm.ExecuteNonQuery();
            conn.Close();
        }
        public void removeThis()
        {
            SqlConnection conn = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\ArmanS\Desktop\WPF\WPF Project - Bookstore\WPF Project - Bookstore\DB\RRDB.mdf;Integrated Security=True;Connect Timeout=30");
            conn.Open();
            string command = "select * from books";
            SqlDataAdapter adapter = new SqlDataAdapter(command, conn);
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);
            for (int i = 0; i < dataTable.Rows.Count; i++)
            {
                if (dataTable.Rows[i][6].ToString() == this.Username)
                {
                    string newcommand;
                    newcommand = "update books SET delay = '" + 0 + "', borrowed_username = '" + (-1) + "' , availability = '" + true + "' where id = '" + dataTable.Rows[i][0] + "'";
                    SqlCommand comm2 = new SqlCommand(newcommand, conn);
                    comm2.ExecuteNonQuery();
                }
            }
            SqlCommand comm1 = new SqlCommand(command, conn);
            comm1.ExecuteNonQuery();
            command = "delete from users where username = '" + this.Username + "' ";
            SqlCommand comm = new SqlCommand(command, conn);
            comm.ExecuteNonQuery();
            conn.Close();
        }
    }
}
