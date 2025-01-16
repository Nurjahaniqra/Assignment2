using Assignment2.Models;
using Microsoft.Ajax.Utilities;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.WebPages;
using System.Xml.Linq;

namespace Assignment2.Controllers
{
    public class AuthController : Controller
    {
       
        public AuthController()
        {
            
        }
        
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(string txtUserName,string txtEmail, string txtPassword, string txtConfirmPassword)
        {
            if (!Regex.IsMatch(txtEmail, @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"))
            {
                ViewBag.Message = "Invalid email format.";
                return View();
            }
            if (string.IsNullOrEmpty(txtUserName) || string.IsNullOrEmpty(txtEmail) ||
                 string.IsNullOrEmpty(txtPassword) || string.IsNullOrEmpty(txtConfirmPassword))
            {
                ViewBag.Message = "All fields are required.";
                return View();
            }
            if (txtPassword != txtConfirmPassword)
            {
                ViewBag.Message = "Passwords do not match.";
                return View();
            }

            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Check if username or email already exists
                string checkQuery = $"SELECT COUNT(1) FROM Auth WHERE UserName ='{txtUserName}' OR UserEmail ='{txtEmail}'";
                using (SqlCommand checkCommand = new SqlCommand(checkQuery, connection))
                {
                    checkCommand.Parameters.AddWithValue("@UserName", txtUserName);
                    checkCommand.Parameters.AddWithValue("@UserEmail", txtEmail);

                    int exists = (int)checkCommand.ExecuteScalar();

                    if (exists > 0)
                    {
                        ViewBag.Message = "Username or email already exists. Please choose a different one.";
                        return View();
                    }
                }

                // Insert new user into the database
                string insertQuery = $"INSERT INTO Auth VALUES('{txtUserName}','{txtEmail}','{txtPassword}','{txtConfirmPassword}')";
                using (SqlCommand insertCommand = new SqlCommand(insertQuery, connection))
                {
                    insertCommand.Parameters.AddWithValue("@UserName", txtUserName);
                    insertCommand.Parameters.AddWithValue("@UserEmail", txtEmail);
                    insertCommand.Parameters.AddWithValue("@Password", txtPassword);
                    insertCommand.Parameters.AddWithValue("@ConfirmPassword", txtConfirmPassword);

                    insertCommand.ExecuteNonQuery();
                }
            }

            ViewBag.Message = "Registration successful!";
            return View();

            //if (string.IsNullOrEmpty(txtUserName) || string.IsNullOrEmpty(txtEmail) ||string.IsNullOrEmpty(txtPassword) ||string.IsNullOrEmpty(txtConfirmPassword))
            //{
            //    ViewBag.Message("All fields are required.");
            //    return View();
            //}
            //string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            //using (SqlConnection connection = new SqlConnection(connectionString))
            //{
            //    string checkQuery = $"SELECT COUNT(1) FROM Auth WHERE UserName = '{txtUserName}' OR UserEmail ='{txtEmail}'";

            //    if (checkQuery.Count() > '1')
            //    {
            //        ViewBag.Message("Username or email already exists. Please choose a different one.");
            //        return View();
            //    }

            //    else
            //    {
            //    //string query = $"Insert Into Auth values({txtUserName},{txtEmail},{txtPassword},{txtConfirmPassword})";
            //    string query1 = $"INSERT INTO Auth VALUES('{txtUserName}','{txtEmail}','{txtPassword}','{txtConfirmPassword}')";

            //    Console.WriteLine(query1);


            //    SqlCommand command = new SqlCommand(query1, connection)
            //    {
            //        CommandType = CommandType.Text
            //    };
            //    //command.Parameters.AddWithValue("@UserName", txtUserName);
            //    //command.Parameters.AddWithValue("@UserEmail", txtEmail);
            //    //command.Parameters.AddWithValue("@PassWord", txtPassword);
            //    //command.Parameters.AddWithValue("@ConfirmPassWord", txtConfirmPassword);
            //    connection.Open();
            //    command.ExecuteNonQuery();
            //    }
            //}
            //return View();
        }

        public ActionResult Login()
        {            
            return View();
        }
        [HttpPost]
        public ActionResult Login(string txtEmail, string txtPassword)
        {
                     
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string query = "PrcLogingUser";
                    SqlCommand command = new SqlCommand(query, connection)
                    {
                        CommandType = CommandType.StoredProcedure
                    };
                    
                    command.Parameters.AddWithValue("@Email", txtEmail);
                    command.Parameters.AddWithValue("@Password", txtPassword); 
                    connection.Open();                    
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.HasRows)
                    {
                        reader.Read();
                        string storeduserEmail = reader["userEmail"].ToString();
                        string storedPassword = reader["Password"].ToString(); 
                                                                               
                        if (storeduserEmail == txtEmail && storedPassword == txtPassword) 
                        {
                            ViewBag.Message = "Login successful!";
                            
                            return RedirectToAction("Contact", "Home"); 
                        }
                    }

                    ViewBag.Message = "Invalid email or password.";
                    return View();
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "An error occurred: " + ex.Message;
                    return View();
                }
            }
           
        }
        [HttpGet]
        public ActionResult ForgetPassWord()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassWord(string txtEmail, string txtPassword, string txtConfirmPassword)
        {
           
            if (string.IsNullOrEmpty(txtEmail) || string.IsNullOrEmpty(txtPassword) || string.IsNullOrEmpty(txtConfirmPassword))
            {
                ViewBag.Message = "All fields are required.";
                return View();
            }

            if (txtPassword != txtConfirmPassword)
            {
                ViewBag.Message = "Passwords do not match.";
                return View();
            }

            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    string query = $"Update A Set A.password='{txtPassword}',A.ConfirmPassWord='{txtConfirmPassword}' from Auth as A Where A.UserEmail='{txtEmail}'";                  
                    SqlCommand command = new SqlCommand(query, connection); 
                    CommandType commandType = CommandType.Text;

                    connection.Open();
                    command.ExecuteNonQuery();                    

                }
                catch (Exception ex)
                {
                    ViewBag.Message = "An error occurred: " + ex.Message;
                    return View();
                }
            
            }
            return RedirectToAction("About", "Home");
        }
    }
}