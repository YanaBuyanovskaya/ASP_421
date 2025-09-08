using ASP_421.Data;
using ASP_421.Models.User;
using ASP_421.Services.KDF;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using System.Text.Json;


namespace ASP_421.Controllers
{
    public class UserController
        (DataContext dataContext, 
        IKDFService kdfService) : Controller
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IKDFService _kdfService= kdfService;

        const String RegisterKey = "RegisterFormModel";
        public IActionResult SignUp()
        {
            UserSignUpViewModel viewModel = new();

            if (HttpContext.Session.Keys.Contains(RegisterKey))
            {
                UserSignUpFormModel formModel =
                    JsonSerializer.Deserialize<UserSignUpFormModel>(
                        HttpContext.Session.GetString(RegisterKey)!)!;

                viewModel.FormModel = formModel;
                viewModel.ValidationErrors = ValidateSignUpForm(formModel);

                if (viewModel.ValidationErrors.Count == 0)
                {
                    var user = new Data.Entities.User
                    {
                        Id = Guid.NewGuid(),
                        Name = formModel.Name,
                        Email = formModel.Email,
                        BirthDate = formModel.Birthday,
                        RegisteredAt = DateTime.Now,
                        DeletedAt = null,
                    };

                    var salt = Guid.NewGuid().ToString();
                    var userAccess1 = new Data.Entities.UserAccess
                    {
                        Id = Guid.NewGuid(),
                        UserId = user.Id,
                        RoleId = "Guest",
                        Login = formModel.Login,
                        Salt = salt,
                        Dk = _kdfService.DK(formModel.Password, salt),
                    };

                    _dataContext.Users.Add(user);
                    _dataContext.UserAccesses.Add(userAccess1);
                    try
                    {
                        _dataContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        viewModel
                            .ValidationErrors ??= new Dictionary<string, string>();
                        viewModel.ValidationErrors["__General"] = ex.Message;
                    }

                    HttpContext.Session.Remove(RegisterKey);
                }
                return View(viewModel);
            }
            return View(viewModel);
        }

        [HttpPost]
        public RedirectToActionResult Register(UserSignUpFormModel formModel)
        {
            HttpContext.Session.SetString(
                RegisterKey,
                JsonSerializer.Serialize(formModel));

            return RedirectToAction(nameof(SignUp));
        }
        
        private Dictionary<String, String> 
            ValidateSignUpForm(UserSignUpFormModel formModel)
        {
            Dictionary<String, String> res = new();

            if(String.IsNullOrEmpty(formModel.Name))
            {
                res[nameof(formModel.Name)] = "Ім'я не може бути порожнім!";
            }

            if (!char.IsUpper(formModel.Name[0]))
            {
                res[nameof(formModel.Name)] = "Ім'я має починатися з великої літери!";
            }

            if(!formModel.Name.All(char.IsLetter))
            {
                res[nameof(formModel.Name)] = "Ім'я має містити тільки літери!";
            }

            if((formModel.Password != formModel.Repeat))
            {
                res[nameof(formModel.Repeat)] = "Паролі не збігаються!";
            }

            if((formModel.Login?.Contains(':') ?? false))
            {
                res[nameof(formModel.Login)] = "У логіні не допускаються ':' (двокрапка)!";
            }

            else if(_dataContext.UserAccesses.Any(ua=>ua.Login==formModel.Login))
            {
                res[nameof(formModel.Login)] = "Логін вже у вжитку!";
            }


            //Email
            if (String.IsNullOrWhiteSpace(formModel.Email))
            {
                res[nameof(formModel.Email)] = "Пошта має бути заповлена!";
            }
            else if(!formModel.Email.Contains('@'))
                {
                res[nameof(formModel.Email)] = "Пошта має бути зі знаком @!";
            }



            //Password
            if ((formModel.Password.Length < 12))
            {
                res[nameof(formModel.Password)] = "Пароль має містити 12 і більше символів!";
            }

            if((!formModel.Password.Any(char.IsUpper)))
            {
                res[nameof(formModel.Password)] = "Пароль має містити хоча б одну велику літеру!";
            }

            if((!formModel.Password.Any(char.IsLower)))
            {
                res[nameof(formModel.Password)] = "Пароль має містити хоча б одну маленьку літеру!";
            }

            if((!formModel.Password.Any(char.IsDigit)))
            {
                res[nameof(formModel.Password)] = "Пароль має містити хоча б один символ! ";
            }

            if ((!formModel.Password.Any(char.IsNumber)))
            {
                res[nameof(formModel.Password)] = "Пароль має містити хоча б одну цифру!";
            }
            




            return res;
        }
    
    
    
    }
}



/*   Browser             НЕПРАВИЛЬНО                 Server
 *  POST name=User -------------------------->
 *       <---------------------------------------HTML
 *      Оновити: POST name=User ------------> ?Conflict - повторні дані
 */


/*   Browser             ПРАВИЛЬНО                 Server
 *  POST /Register name=User ----------------------->  зБЕРІГАЄ ДАНІ(у сесії)
 *       <------------------302----------------------  Redirect /SignUp
 *       GET / SignUp --------------------------->  Відновлює та оброблює дані
 *                  <----------200-------------------  HTML
 *      Оновити: GET / SignUp ------------------->  Немає конфлікту       
 */
