using Amazon.Runtime.Internal;
using Azure.Core;
using JurayKV.Application.Commands.IdentityCommands.UserCommands;
using JurayKV.Application.Commands.NotificationCommands;
using JurayKV.Application.Commands.TransactionCommands;
using JurayKV.Application.Commands.UserManagerCommands;
using JurayKV.Application.Commands.WalletCommands;
using JurayKV.Application.Queries.IdentityQueries.UserQueries;
using JurayKV.Application.Queries.SettingQueries;
using JurayKV.Application.Queries.TransactionQueries;
using JurayKV.Application.Queries.UserManagerQueries;
using JurayKV.Application.Queries.WalletQueries;
using JurayKV.Application.Services;
using JurayKV.Domain.Aggregates.IdentityAggregate;
using JurayKV.Domain.Aggregates.WalletAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static JurayKV.Domain.Primitives.Enum;

namespace JurayKV.UI.Areas.Auth.Pages.Account
{

    [AllowAnonymous]
    public class ComfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMediator _mediator;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IWalletRepository _walletRepository;
        public ComfirmationModel(UserManager<ApplicationUser> userManager, IMediator mediator, SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, IWalletRepository walletRepository)
        {
            _userManager = userManager;
            _mediator = mediator;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
            _walletRepository = walletRepository;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        /// [bind
        /// 
        [BindProperty]
        public Guid Xtxnt { get; set; }
        [BindProperty]
        public string One { get; set; }
        
        [BindProperty]
        public int NotificationNumber { get; set; }
        public async Task<IActionResult> OnGetAsync(string xcode, string xmal, string txtd)
        {
            if (xmal == null)
            {
                return RedirectToPage("/Index");
            }
            if (txtd == null)
            {
                return RedirectToPage("/Index");

            }
            var data = await _userManager.FindByIdAsync(txtd);

            if (data == null)
            {
                return NotFound($"Unable to load user with email '{xmal}'.");
            }

            Xtxnt = data.Id;
            TempData["codetype"] = xcode;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var identityResult = await _userManager.FindByIdAsync(Xtxnt.ToString());
                    if(identityResult.EmailConfirmed == true) {
                        
                        return RedirectToPage("./Login");
                        }
                    GetEmailVerificationCodeQuery command = new GetEmailVerificationCodeQuery(identityResult.Id.ToString());

                    EmailVerificationCode result = await _mediator.Send(command);
                    //
                    string VerificationCode = One;
                     
                    VerificationCode = VerificationCode.Replace(" ", "");
                    result.Code = result.Code.Replace(" ", "");
                    if (result.Code == VerificationCode)
                    {
                        identityResult.EmailConfirmed = true;
                        await _userManager.UpdateAsync(identityResult);

                        //if the referral is not null, credit the referral
                        //get user by id
                        GetUserManagerByPhoneQuery getuserbyphonecommand = new GetUserManagerByPhoneQuery(identityResult.RefferedByPhoneNumber);
                        var UserWHoReferredTheseAccount = await _mediator.Send(getuserbyphonecommand);
                        if(UserWHoReferredTheseAccount != null)
                        {
                            //get settings
                            GetSettingDefaultQuery settingscommand = new GetSettingDefaultQuery();
                            var settingData = await _mediator.Send(settingscommand);

                          
                            //if transaction is debit.
                            GetWalletUserByIdQuery walletcommand = new GetWalletUserByIdQuery(UserWHoReferredTheseAccount.Id);
                            var getwallet = await _mediator.Send(walletcommand);

                            //create the transaction
                              
                            CreateTransactionCommand transactioncreatecommand = new CreateTransactionCommand(getwallet.Id, getwallet.UserId, "Referral Bonus",
                                settingData.DefaultReferralAmmount,
                            TransactionTypeEnum.Credit, EntityStatus.Successfull, Guid.NewGuid().ToString(), "Referral Bonus", Guid.NewGuid().ToString() + "-REFERRAL");
                            var transaction = await _mediator.Send(transactioncreatecommand);
                            //GET transaction information to update wallet
                            //get the transaction by id
                            GetTransactionByIdQuery gettranCommand = new GetTransactionByIdQuery(transaction);
                            var thetransaction = await _mediator.Send(gettranCommand);
                            //update walet
                            getwallet.Amount = getwallet.Amount + settingData.DefaultReferralAmmount;
 
                            var loguserId = _httpContextAccessor.HttpContext?.User?.Identity?.Name;
                            getwallet.Log = getwallet.Log + "<br>Referral Bonus- Wallet Update from " + thetransaction.Description + " " + thetransaction.Id + " ::Amount: " + thetransaction.Amount + " ::Balance: " + getwallet.Amount + " :: Date: " + getwallet.LastUpdateAtUtc + ":: Loggedin User: " + loguserId;
                            //getwallet = null;
                            UpdateWalletCommand updatewalletcommand = new UpdateWalletCommand(getwallet.UserId, "Validate Transaction", getwallet.Log, getwallet.Amount);
                            await _mediator.Send(updatewalletcommand);
                        }

                        //if (DateTime.UtcNow > result.SentAtUtc.AddMinutes(5))
                        //{
                        //    TempData["error"] = "The code is expired.";
                        //    return Page();
                        //}
                        //UpdateEmailVerificationCodeCommand verificationcommand = new UpdateEmailVerificationCodeCommand(result.Email, result.PhoneNumber, result.Id.ToString(), result.Code, result.Id, DateTime.UtcNow);
                        //bool verificationresult = await _mediator.Send(verificationcommand);
                        await _signInManager.SignInAsync(identityResult, true);
                        LastLoginCommand lst = new LastLoginCommand(identityResult.Id.ToString());
                        await _mediator.Send(lst);
                        return RedirectToPage("/Account/Index", new { area = "User" });
                    }
                    else
                    {
                        TempData["error"] = "Invalid Code";
                        return Page();
                    }

                }
                catch (Exception exception)
                {

                    return StatusCode(StatusCodes.Status500InternalServerError);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

    }
}
