using ASP_421.Data;
using ASP_421.Models.User;
using ASP_421.Services.KDF;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Primitives;
using System.Text;
using System.Text.Json;


namespace ASP_421.Controllers
{
    public class UserController
        (DataContext dataContext, 
        IKDFService kdfService) : Controller
    {
        private readonly DataContext _dataContext = dataContext;
        private readonly IKDFService _kdfService= kdfService;
        
        private const String SessionUserKey = "SessionUser"; 

        const String RegisterKey = "RegisterFormModel";

        [HttpGet]
        public IActionResult Cabinet(Guid? id)
        {
            var actualId = id ?? GetIdFromSession();
            if (actualId == null || actualId == Guid.Empty)
                return Unauthorized();

            var user = _dataContext.Users.FirstOrDefault(u => u.Id == actualId.Value);
            if (user == null) return NotFound();

            var vm = new ASP_421.Models.User.UserCabinetViewModel
            { 
                User = user,
                ValidationErrors = null,
                IsEditing = false
            };
            return View(vm);

        }

        private Guid? GetIdFromSession()
        {
            var s = HttpContext.Session.GetString(SessionUserKey);
            if (string.IsNullOrWhiteSpace(s)) return null;
            var dto = JsonSerializer.Deserialize<SessionUserDto>(s);
            return dto?.Id;
        }
        private record SessionUserDto(Guid Id, string Name, string Email);
        public IActionResult Profile([FromRoute] String id)
        {
            UserProfileViewModel viewModel = new();

            viewModel.User = _dataContext
                .UserAccesses
                .Include(ua => ua.User)
                .AsNoTracking()
                .FirstOrDefault(ua => ua.Login == id)
                ?.User;

            String? authUserId = HttpContext
                .User
                .Claims
                .FirstOrDefault(c => c.Type == "Id")
                ?.Value;

            viewModel.IsPersonal = authUserId != null &&
                authUserId == viewModel.User?.Id.ToString();

            return View(viewModel);
        }

        [HttpPatch]
        public JsonResult Update([FromBody] JsonElement json)
        {
           if(json.GetPropertyCount()==0)
            {
                return Json(new
                {
                    Status = 401,
                    Data = "Missing Authorization header "
                });
            }
           if(!(HttpContext.User.Identity?.IsAuthenticated ?? false))
            {
                return Json(new
                {
                    Status = 401,
                    Data = "Unauthorized"
                });
            }
            String id = HttpContext.User.Claims.First(c => c.Type == "Id").Value;
            Data.Entities.User user = _dataContext
                 .Users
                 .Find(Guid.Parse(id))!;

            if(json.TryGetProperty("Name", out JsonElement name))
            {
                user.Name = name.GetString()!;
            }

            if (json.TryGetProperty("Email", out JsonElement email))
            {
                user.Email = email.GetString()!;
            }
            _dataContext.SaveChanges();
            return Json(new
            {
                Status = 200,
                Data = "Ok"
            });
        }

        [HttpDelete]
        public JsonResult Delete(Guid id)
        {//перевірити чи користувач авторизований
            //визначити його ай ді та відшукати об'єкт БД
            //видалити персональні дані
            //встановити дату видалення DeletedAt у поточний момент
            //save changes 

            var user = _dataContext.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                Response.StatusCode = 401;
                return Json(new { status = "not_found", id });
            }

            user.DeletedAt = DateTime.UtcNow;
            _dataContext.SaveChanges();

            HttpContext.Session.Remove(SessionUserKey);
                

            return Json(new
            {
                Status = 200,
                Data = "Ok",
                deletedAt = user.DeletedAt
            });
        }
        public JsonResult SignIn()
        {
            //Authorization: Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==


            String header = // Basic QWxhZGRpbjpvcGVuIHNlc2FtZQ==
                    HttpContext.Request.Headers.Authorization.ToString();
            if (String.IsNullOrEmpty(header))
            {
                return Json(new
                {
                    Status = 401,
                    Data = "Missing Authorization header "
                });
            }
            String scheme = "Basic ";
            if (!header.StartsWith(scheme))
            {
                return Json(new
                {
                    Status = 401,
                    Data = "Invalid scheme. Required: " + scheme
                });
            }

            String credentials = // QWxhZGRpbjpvcGVuIHNlc2FtZQ==
                header[scheme.Length..];
            String userPass =    // Aladdin: open sesame
                Encoding.UTF8.GetString(
                Convert.FromBase64String(credentials));

            String[] parts = userPass.Split(':', 2);
            String login = parts[0];        //Aladdin
            String password = parts[1];     //open sesame

            var userAccess =
                _dataContext
                .UserAccesses
                .AsNoTracking()          // не моніторити зміни
                .Include(ua => ua.User) //заповнення навігаційної властивості
                .FirstOrDefault
                (ua => ua.Login == login);

            if (userAccess == null)
            {
                return Json(new
                {
                    Status = 401,
                    Data = "Credentials rejected"
                });
            }

            if (_kdfService.DK(password, userAccess.Salt) != userAccess.Dk)
            {
                return Json(new
                {
                    Status = 401,
                    Data = "Credentials rejected."

                });
            }

            // користувач пройшов автентифікацію, зберігаємо у сесії
            HttpContext.Session.SetString(
                "SignIn",
                JsonSerializer.Serialize(userAccess));

            return Json(new
            {
                Status = 200,
                Data = "Authorized"
            });
        }
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

            else if(_dataContext.UserAccesses
                .Include(ua=>ua.User)
                .Any(ua=>ua.Login== formModel.Login && ua.User.DeletedAt == null))
            {
                res[nameof(formModel.Login)] = "Логін вже у вжитку!";
            }



            //Password
            if ((formModel.Password.Length < 12))
            {
                res[nameof(formModel.Password)] = "Пароль має містити 12 і більше символів!";
            }

            if(!formModel.Password.Any(char.IsUpper))
            {
                res[nameof(formModel.Password)] = "Пароль має містити хоча б одну велику літеру!";
            }

            if(!formModel.Password.Any(char.IsLower))
            {
                res[nameof(formModel.Password)] = "Пароль має містити хоча б одну маленьку літеру!";
            }

            if(!formModel.Password.Any(char.IsDigit))
            {
                res[nameof(formModel.Password)] = "Пароль має містити хоча б один символ! ";
            }

            if (!formModel.Password.Any(char.IsNumber))
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
