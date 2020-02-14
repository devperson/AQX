using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;    

using UIKit;
using Foundation;
using MessageUI;

using Aquamonix.Mobile.Lib.Environment;
using Aquamonix.Mobile.IOS.Utilities.WebSockets;
using Aquamonix.Mobile.IOS.Utilities;
using Aquamonix.Mobile.Lib.Utilities.WebSockets;
using Aquamonix.Mobile.Lib.Database;
using Aquamonix.Mobile.Lib.Utilities;
using Aquamonix.Mobile.Lib.Services;
using Aquamonix.Mobile.Lib.Domain;
using Aquamonix.Mobile.Lib.Domain.Responses;
using Aquamonix.Mobile.IOS.Views; 
using Aquamonix.Mobile.IOS.UI;

namespace Aquamonix.Mobile.IOS.ViewControllers
{
    /// <summary>
    /// User config & login screen. 
    /// </summary>
    public partial class UserConfigViewController : ListViewControllerBase
    {
        private static readonly bool ValidateUriFormat = true;
        private static readonly bool AllowSubmitWithEmptyUriField = false;

        private static UserConfigViewController _instance;
        private UIViewController _caller;
        private bool _connectingManually = false;

        private UserConfigViewController() : base()
        {
            this.Initialize();
        }

        public static UserConfigViewController CreateInstance(UIViewController caller = null)
        {
            ExceptionUtility.Try(() =>
            {
                if (_instance != null)
                {
                    _instance.Dispose();
                    _instance = null;
                }
            });

            _instance = new UserConfigViewController();
            _instance._caller = caller;
            //_instance.ClearAllAppDataExceptUserInfo(); 

            return _instance;
        }

        protected override void DoLoadData()
        {
            ExceptionUtility.Try(() =>
            {
                base.DoLoadData();
                List<ConfigSettingViewModel> settings = new List<ConfigSettingViewModel>();

                string username = String.Empty;
                string password = String.Empty;
                string server = String.Empty;

                if (User.Current != null)
                {
                    username = User.Current.Username;
                    password = User.Current.Password;
                    server = User.Current.ServerUri;
                }
                else
                {
                    #if DEBUG
                    {
                        //username = "demo@aquamonix.com.au";
                       // password = "12341234";
                        //  server = "ws://172.16.16.243:8080/service";
                        //server = "wss://raincloud.aquamonix.com.au:444/service";
                        //server = "ws://127.0.0.1:8080/service";

                    }
                    #endif
                }

                //if no server, default to default
                if (String.IsNullOrEmpty(server))
                    server = AppSettings.GetAppSettingValue(AppSettings.DefaultAppServerUriAppSettingName, String.Empty); 

                this._navBarView.ShowHideCancelButton(User.Current != null && User.Current.HasConfig && DataCache.HasDevices);

                settings.Add(new ConfigSettingViewModel() { ControlType = ConfigSettingControlType.TextField, Type = ConfigSettingType.Username, CurrentValue = username });
                settings.Add(new ConfigSettingViewModel() { ControlType = ConfigSettingControlType.TextField, Type = ConfigSettingType.Password, CurrentValue = password });
                settings.Add(new ConfigSettingViewModel() { ControlType = ConfigSettingControlType.TextField, Type = ConfigSettingType.ServerUri, CurrentValue = ServerAliases.UriToAlias(server) });

                this._logoutButton.Hidden = true;
                if (User.Current != null && User.Current.HasConfig && ServiceContainer.UserService.IsConnected)
                {
                    this._logoutButton.Hidden = false;
                    //settings.Add(new ConfigSettingViewModel() { ControlType = ConfigSettingControlType.Button, Type = ConfigSettingType.LogoutButton, Valid = true });
                }

                foreach (var setting in settings)
                {
                    if (setting.Type == ConfigSettingType.LogoutButton)
                    {
                        setting.OnValueChanged = (obj) =>
                        {
                            this.Logout();
                        };
                    }
                    else
                        setting.OnValueChanged = (obj) => { this.ValidateForm(); };
                }

                this._tableViewController.LoadData(settings);
                this._tableViewController.SetCallbacks(async () =>
                {
                    await ServiceContainer.UserService.LogOut();
                    SubmitForm();
                });

                this.ValidateForm();
            });
        }

        public void ShowError(ConnectionResponse response, bool showError = false)
        {
            if (response != null)
            {
                if (!response.IsSuccessful)
                {
                    if (response.HasError)
                    {
                        if (showError)
                        {
                            MainThreadUtility.InvokeOnMain(() => {
                                //AlertUtility.ShowAppError(response.ErrorBody);
                            });
                        }
                    }
                    else {
                        this.ShowError(StringLiterals.ConnectionError);
                    }
                }
            }
            else
                this.ShowError(StringLiterals.ConnectionError);
           }

        protected override void InitializeViewController()
        {
            base.InitializeViewController();

            ExceptionUtility.Try(() =>
            {
                this.NavigationBarView = _navBarView;
                this.TableViewController = this._tableViewController;
                this.TopMargin = 50;

                this._navBarView.OnDoneClicked = () =>
                {
                    //ReconnectingOverlay.Instance.Show(); 
                    this.SubmitForm();
                };

                this._navBarView.OnCancelClicked = () =>
                {
                    if (this._caller is GlobalAlertListViewController)
                        this.NavigateAlerts();
                    else
                        this.NavigateHome(); 
                };

                this._logoutButton.TouchUpInside += (o, e) =>
                {
                    this.Logout();
                };

                this._supportButton.TouchUpInside += (o, e) =>
                {
                    this.ShowEmail();
                };


                this.TabBar.SelectTab(2);
                this.ValidateForm();
            });
           }

        public override void NavigateUserConfig()
        {
            //Do nothing, cause we're already here 
        }


        protected override void OnAuthFailure()
        {
            //do nothing on purpose 
        }

        protected override void OnReconnected()
        {
            if (this.IsShowing)
            {
                if (!ConnectionManager.InitialConnection || _connectingManually)
                {
                    _connectingManually = false;
                    ConnectionManager.InitialConnection = true;

                    MainThreadUtility.InvokeOnMain(() => 
                    {
                        //only save the changes after successful login 
                        User.Current.Save();

                        //navigate to device list 
                        this.NavigateTo(DeviceListViewController.CreateInstance(), inCurrentNavController: false);
                    });
                }
            }
        }

        protected override void ShowReconBar(string text = null)
        {
            //DO NOTHING; don't want the banner showing on this screen
            //base.ShowReconnectingBarView(text);
            //_tableViewController.TableView.ContentInset = new UIEdgeInsets(50, 0, 0, 0);
        }

        protected override void HandleViewWillAppear(bool animated)
        {
            base.HandleViewWillAppear(animated);

            //if we're reconnecting at this point, cancel 
            ConnectionManager.CancelReconnecting();
        }


        private void ValidateForm()
        {
            bool valid = this._tableViewController.Validate();

            this._navBarView.EnableDisableDoneButton(valid); 
        }

        private async void SubmitForm()
        {
            try
            {
                _connectingManually = true; 

                if (this._navBarView.DoneButtonEnabled)
                {
                    //var oldUsername = User.Current?.Username;
                    //var oldPassword = User.Current?.Password;
                    //var oldServerUri = User.Current?.ServerUri;

                    var username = this._tableViewController.Username;
                    var password = this._tableViewController.Password;
                    var serverUri = this._tableViewController.ServerUri;

                    //set default if no server uri provided
                    if (String.IsNullOrEmpty(serverUri))
                    {
                        serverUri = AppSettings.GetAppSettingValue(AppSettings.DefaultAppServerUriAppSettingName, String.Empty);
                        this._tableViewController.ServerUri = serverUri;
                    }

                    //translate server alias to a uri
                    serverUri = ServerAliases.AliasToUri(serverUri);

                    bool newUser = false;
                    bool newServer = false;

                    if (User.Current == null)
                    {
                        User.Current = new User();
                    }
                    else
                    {
                        //check if current user is different from prev user 
                        newUser = ((User.Current.Username != null) && User.Current.Username.Trim().ToLower() != username.Trim().ToLower());
                        newServer = ((User.Current.ServerUri != null) && User.Current.ServerUri.Trim().ToLower() != serverUri.Trim().ToLower());
                    }

                    if (newServer || newUser)
                    {
                        await this.LogoutInternal(); 
                    }

                    User.Current.Username = username;
                    User.Current.Password = password;
                    User.Current.ServerUri = serverUri;

                    if (User.Current != null)
                    {
                        await ProgressUtility.SafeShow("Connecting to Server", async () =>
                        {
                            //if reconnect process is running, cancel it 
                            if (ConnectionManager.IsReconnecting)
                            {
                                await ConnectionManager.CancelReconnecting();
                            }

                            //here, if we are voluntarily connecting to a new server or new user, we must suppress the automatic reconnect attempt on closing connection
                            if (newUser || newServer)
                                ConnectionManager.Deinitialize();

                            WebSocketsClient.ResetWebSocketsClientUrl(new WebSocketsClientIos(), serverUri);
                            ConnectionManager.Initialize();

                            var deviceListVc = DeviceListViewController.CreateInstance();

                            //test connection 
                            var response = await ServiceContainer.UserService.RequestConnection(username, password);
                            ProgressUtility.Dismiss();

                            this.ShowError(response);

                            if (response != null && response.IsSuccessful)
                            {
                                //only save the changes after successful login 
                                //User.Current.Save();

                                //this.NavigateTo(deviceListVc, inCurrentNavController: false);
                                //PresentViewController(new AquamonixNavController(deviceListVc), true, null);
                                LogUtility.Enabled = true;
                            }
                            else
                            {
                                if (response?.ErrorBody != null && response.ErrorBody.ErrorType == ErrorResponseType.ConnectionTimeout)
                                {
                                    if (DataCache.HasDevices)
                                    {
                                        deviceListVc.ShowConnectionError = true;
                                        this.NavigateTo(deviceListVc, inCurrentNavController: false);
                                    }
                                    else
                                    {
                                        AlertUtility.ShowErrorAlert(StringLiterals.ConnectionError, StringLiterals.UnableToEstablishConnection);
                                        ConnectionManager.CancelReconnecting(); 
                                    }
                                }
                                // Edited //
                                if (response?.ErrorBody != null && response.ErrorBody.ErrorType == ErrorResponseType.AuthFailure)
                                {
                                    AlertUtility.ShowErrorAlert(StringLiterals.ConnectionError, StringLiterals.AuthFailureMessage);
                                    Caches.ClearCachesForAuthFailure();
                                }

                                this.LoadData();
                            }
                        });
                    }
                }
            }
            catch (Exception e)
            {
                LogUtility.LogException(e);
            }
        }

        private void ShowError(string errorMsg)
        {
            AlertUtility.ShowAlert(StringLiterals.Error, errorMsg);
        }

        private void Logout()
        {
            ProgressUtility.SafeShow("Logging Out", async () =>
            {
                await LogoutInternal(); 
            });
        }

        private async Task LogoutInternal()
        {
            try
            {
                ConnectionManager.Deinitialize();
                await ServiceContainer.UserService.LogOut();
                Caches.ClearCachesForLogout();

            #if DEBUG
                //User.Current.Password = password;
            #endif
                User.Current.Save();

                MainThreadUtility.InvokeOnMain(() =>
                {
                    ProgressUtility.Dismiss();
                    this.LoadData();
                    LogUtility.Enabled = false;
                });
            }
            catch(Exception e)
            {
                LogUtility.LogException(e); 
            }
        }

        private void ShowEmail()
        {
            ExceptionUtility.Try(() =>
            {
                #if DEBUG
                string recipients = "john.kosinski@gmail.com"; 
                #else
                string recipients = AppSettings.GetAppSettingValue<string>(AppSettings.LogEmailRecipientsAppSettingName, "appsupport@aquamonix.com.au");
                #endif

                if (MFMailComposeViewController.CanSendMail)
                {
                    LogUtility.LogMessage("Preparing to send diagnostic email to " + recipients, LogSeverity.Info); 
                    MFMailComposeViewController mailVc = new MFMailComposeViewController();
                    mailVc.SetToRecipients(new string[] { recipients });

                    string subject = StringLiterals.SupportRequestSubject;
                    //if (User.Current != null)
                    //    subject += " " + User.Current.Username;

                    mailVc.SetSubject(subject);

                    string message = StringLiterals.EmailText + this.GetDeviceInfoAsString();

                    mailVc.SetMessageBody(message, false);

                    mailVc.Finished += (object s, MFComposeResultEventArgs args) =>
                    {
                        LogUtility.LogMessage("Diagnostic email sent to " + recipients, LogSeverity.Info);
                        System.Diagnostics.Debug.WriteLine(args.Result.ToString());
                        args.Controller.DismissViewController(true, null);
                    };

                    string logData = LogUtility.GetLogData();
                    mailVc.AddAttachmentData(NSData.FromString(logData), "text/plain", "logfile.txt");

                    this.NavigationController.PresentViewController(mailVc, true, () => { });
                }
                else
                {
                    AlertUtility.ShowAlert(
                        StringLiterals.NoMailSupportAlertTitle, 
                        String.Format(StringLiterals.NoMailSupportAlertMessage, recipients)
                    );
                }
            });
        }

        private string GetDeviceInfoAsString()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            ExceptionUtility.Try(() =>
            {
                sb.Append(VersionUtility.GetVersionString());
                sb.Append("\n");
                sb.Append("\n");

                sb.Append("Device Model: ");
                sb.Append(UIDevice.CurrentDevice.Model);
                sb.Append("\n");

                sb.Append("Sys Version: ");
                sb.Append(UIDevice.CurrentDevice.SystemVersion);
                sb.Append("\n");

                sb.Append("Sys Name: ");
                sb.Append(UIDevice.CurrentDevice.SystemName);
                sb.Append("\n");

                sb.Append("Name: ");
                sb.Append(UIDevice.CurrentDevice.Name);
                sb.Append("\n");

                if (User.Current != null)
                {
                    sb.Append("Userame: ");
                    sb.Append(User.Current.Username + "(" + User.Current.Name + ")");
                    sb.Append("\n");

                    sb.Append("Server: ");
                    sb.Append(User.Current.ServerUri);
                    sb.Append("\n");
                }

                sb.Append("\n");
            });

            return sb.ToString();
        }


        private class NavBarView : NavigationBarView
        {
            private const int LeftMargin = 16;
            private const int RightMargin = 16; 

            private readonly AquamonixLabel _titleLabel = new AquamonixLabel();
            private readonly UIButton _cancelButton = new UIButton();
            private readonly UIButton _doneButton = new UIButton();
            private Action _onDoneClicked;
            private Action _onCancelClicked;

            public Action OnDoneClicked
            {
                get { return this._onDoneClicked; }
                set { this._onDoneClicked = WeakReferenceUtility.MakeWeakAction(value); }
            }
            public Action OnCancelClicked
            {
                get { return this._onCancelClicked; }
                set { this._onCancelClicked = WeakReferenceUtility.MakeWeakAction(value); }
            }
            public bool DoneButtonEnabled
            {
                get { return this._doneButton.Enabled;}
            }

            public NavBarView() : base(fullWidth:true)
            {
                ExceptionUtility.Try(() =>
                {
                    //title label 
                    this._titleLabel.SetFontAndColor(new FontWithColor(Fonts.SemiboldFontName, Sizes.FontSize8, Colors.StandardTextColor));
                    this._titleLabel.TextAlignment = UITextAlignment.Center;
                    this._titleLabel.SizeToFit();

                    //done button 
                    this._doneButton.SetTitle(StringLiterals.DoneButtonText, UIControlState.Normal);
                    this._doneButton.SetTitle(StringLiterals.DoneButtonText, UIControlState.Disabled);
                    this._doneButton.Font = NavBarButtonsFont.Font;
                    this._doneButton.SetTitleColor(NavBarButtonsFont.Color, UIControlState.Normal);
                    this._doneButton.SetTitleColor(UIColor.Gray, UIControlState.Disabled);
                    this._doneButton.SizeToFit();
                    this._doneButton.TouchUpInside += (o, e) =>
                    {
                        if (this.OnDoneClicked != null)
                            this.OnDoneClicked();
                    };

                    //cancel button 
                    this._cancelButton.SetTitle(StringLiterals.CancelButtonText, UIControlState.Normal);
                    this._cancelButton.Font = NavBarButtonsFont.Font;
                    this._cancelButton.SetTitleColor(NavBarButtonsFont.Color, UIControlState.Normal);
                    this._cancelButton.SizeToFit();
                    this._cancelButton.TouchUpInside += (o, e) =>
                    {
                        if (this.OnCancelClicked != null)
                            this.OnCancelClicked();
                    };

                    this.AddSubviews(_titleLabel, _cancelButton, _doneButton);

                    this.SetText(StringLiterals.UserSettingsTitle);
                });
            }

            public void SetText(string text)
            {
                this._titleLabel.Text = text;
                this._titleLabel.SetFrameHeight(Height);
            }

            public override void LayoutSubviews()
            {
                ExceptionUtility.Try(() =>
                {
                    base.LayoutSubviews();

                    //title label 
                    this._titleLabel.SetFrameLocation(0, 0);
                    this._titleLabel.SetFrameWidth(this.Frame.Width);

                    //cancel button 
                    this._cancelButton.SetFrameHeight(this.Frame.Height);
                    this._cancelButton.SetFrameLocation(LeftMargin, 0);

                    //done button 
                    this._doneButton.SetFrameHeight(this.Frame.Height);
                    this._doneButton.AlignToRightOfParent(RightMargin);
                });
            }

            public void EnableDisableDoneButton(bool enable)
            {
                this._doneButton.Enabled = enable;
            }

            public void ShowHideCancelButton(bool show)
            {
                this._cancelButton.Hidden = (!show);
            }
        }

        private class UserConfigTableViewController : TopLevelTableViewControllerBase<ConfigSettingViewModel, UserConfigTableViewSource>
        {
            public string Username
            {
                get
                {
                    return this.Values.Where((c) => c.Type == ConfigSettingType.Username).FirstOrDefault().CurrentValue.Trim();
                }
                set
                {
                    this.SetSettingValue(ConfigSettingType.Username, value);
                }
            }
            public string Password
            {
                get
                {
                    return this.Values.Where((c) => c.Type == ConfigSettingType.Password).FirstOrDefault().CurrentValue.Trim();
                }
                set
                {
                    this.SetSettingValue(ConfigSettingType.Password, value);
                }
            }
            public string ServerUri
            {
                get
                {
                    return this.Values.Where((c) => c.Type == ConfigSettingType.ServerUri).FirstOrDefault().CurrentValue.Trim();
                }
                set
                {
                    this.SetSettingValue(ConfigSettingType.ServerUri, value); 
                }
            }

            public UserConfigTableViewController() : base()
            {
                ExceptionUtility.Try(() =>
                {
                    TableView.RegisterClassForCellReuse(typeof(UserConfigTableViewCell), UserConfigTableViewCell.TableCellKey);
                });
            }

            public bool Validate()
            {
                bool output = true;

                ExceptionUtility.Try(() =>
                {
                    for (int n = 0; n < this.Values.Count(); n++)
                    {
                        var cell = this.TableView.CellAt(NSIndexPath.FromRowSection(n, 0)) as UserConfigTableViewCell;
                        if (cell != null)
                        {
                            cell.Validate();
                        }
                    }

                    foreach (var setting in this.Values)
                    {
                        if (!setting.Valid)
                        {
                            output = false;
                            break;
                        }
                    }
                });

                return output; 
            }

            public void SetCallbacks(Action onDoneClicked)
            {
                ExceptionUtility.Try(() =>
                {
                    AquamonixTextField textField = null;

                    for (int n = 0; n < this.Values.Count(); n++)
                    {
                        var cell = this.TableView.CellAt(NSIndexPath.FromRowSection(n, 0)) as UserConfigTableViewCell;

                        if (cell != null)
                        {
                            if (cell.TextField != null)
                            {
                                if (textField != null && textField.ReturnKeyType == UIReturnKeyType.Next)
                                    textField.NextControl = cell.TextField;

                                if (cell.TextField.ReturnKeyType == UIReturnKeyType.Done)
                                    cell.TextField.OnDoneClicked = onDoneClicked;

                                textField = cell.TextField;
                            }
                        }
                    }
                });
            }

            protected void SetSettingValue(ConfigSettingType type, string value)
            {
                var source = (this.TableView.Source as UserConfigTableViewSource);
                if (source != null)
                {
                    source.SetSettingValue(this.TableView, type, value); 
                }
            }

            protected override UserConfigTableViewSource CreateTableSource(IList<ConfigSettingViewModel> values)
            {
                return new UserConfigTableViewSource(values);
            }
        }

        private class UserConfigTableViewSource : TableViewSourceBase<ConfigSettingViewModel>
        {
            public UserConfigTableViewSource(IList<ConfigSettingViewModel> values) : base(values) { }

            public void SetSettingValue(UITableView tableView, ConfigSettingType type, string value)
            {
                for (int n = 0; n < this.Values.Count; n++)
                {
                    if (this.Values[n].Type == type)
                    {
                        this.Values[n].CurrentValue = value;

                        MainThreadUtility.InvokeOnMain(() =>
                        {
                            this.GetCellInternal(tableView, NSIndexPath.FromRowSection(n, 0));
                        }); 
                        break;
                    }
                }
            }

            protected override UITableViewCell GetCellInternal(UITableView tableView, NSIndexPath indexPath)
            {
                UserConfigTableViewCell cell = (UserConfigTableViewCell)tableView.DequeueReusableCell(UserConfigTableViewCell.TableCellKey, indexPath);

                ConfigSettingViewModel setting = null;
                if (indexPath.Row < this.Values.Count())
                    setting = this.Values[indexPath.Row];

                //create cell style
                if (setting != null)
                {
                    cell.LoadCellValues(setting);
                }

                return cell;
            }
        }

        private class UserConfigTableViewCell : TableViewCellBase
        {
            public const string TableCellKey = "UserConfigTableViewCell";

            private readonly AquamonixTextField _textField = new AquamonixTextField();
            private readonly AquamonixLabel _label = new AquamonixLabel();
            private readonly UIButton _button = new UIButton();
            private ConfigSettingViewModel _setting;

            public AquamonixTextField TextField
            {
                get {
                    if (this._setting.ControlType == ConfigSettingControlType.TextField)
                        return this._textField;

                    return null;
                }
            }

            public UserConfigTableViewCell(IntPtr handle) : base(handle)
            {
                ExceptionUtility.Try(() =>
                {
                    this._label.SetFontAndColor(LabelFont);
                    this._textField.Font = TextBoxFont.Font;
                    this._textField.TextColor = TextBoxFont.Color;

                    this._textField.OnTextChanged = (s) =>
                    {
                        bool valid = this._textField.Validate();
                        this._setting.Valid = valid;
                        this._setting.CurrentValue = s;
                        if (this._setting.OnValueChanged != null)
                            this._setting.OnValueChanged(s);
                    };

                    this._button.TouchUpInside += (o, e) =>
                    {
                        if (this._setting.OnValueChanged != null)
                            this._setting.OnValueChanged(this._setting.CurrentValue);
                    };

                    this.AddSubviews(_textField, _label, _button);
                });
            }

            public void LoadCellValues(ConfigSettingViewModel setting)
            {
                ExceptionUtility.Try(() =>
                {
                    this._setting = setting;
                    this._label.Hidden = true;
                    this._textField.Hidden = true;
                    this._button.Hidden = true;

                    switch (setting.ControlType)
                    {
                            case ConfigSettingControlType.TextField:
                            this._label.Hidden = false;
                            this._textField.Hidden = false;

                            switch (setting.Type)
                            {
                                case ConfigSettingType.Username:
                                    this._label.Text = StringLiterals.UsernameTextFieldPlaceholder;
                                    this._textField.Placeholder = StringLiterals.UsernameTextFieldPlaceholder;
                                    this._textField.MaxLength = 40;
                                    this._textField.AutocorrectionType = UITextAutocorrectionType.No;
                                    this._textField.AutocapitalizationType = UITextAutocapitalizationType.None;
                                    this._textField.ReturnKeyType = UIReturnKeyType.Next;
                                    this._textField.SecureTextEntry = false;
                                    this._textField.ValidationFunction = (s) =>
                                    {
                                        if (s != null)
                                            s = s.Trim();

                                        if (String.IsNullOrEmpty(s))
                                        {
                                            _textField.TextColor = Colors.ErrorTextColor;
                                            return "Please enter a username";
                                        }
                                        else {
                                            _textField.TextColor = UIColor.DarkGray;
                                        }

                                        return null;
                                    };
                                    break;

                                case ConfigSettingType.Password:
                                    this._label.Text = StringLiterals.PasswordTextFieldPlaceholder;
                                    this._textField.Placeholder = StringLiterals.PasswordTextFieldPlaceholder;
                                    this._textField.MaxLength = 40;
                                    this._textField.ReturnKeyType = UIReturnKeyType.Next;
                                    this._textField.AutocorrectionType = UITextAutocorrectionType.No;
                                    this._textField.AutocapitalizationType = UITextAutocapitalizationType.None;
                                    this._textField.SecureTextEntry = true;
                                    this._textField.ValidationFunction = (s) =>
                                    {
                                        if (s != null)
                                            s = s.Trim();

                                        if (String.IsNullOrEmpty(s))
                                        {
                                            _textField.TextColor = Colors.ErrorTextColor;
                                            return "Please enter a password";
                                        }
                                        else {
                                            _textField.TextColor = UIColor.DarkGray;
                                        }

                                        return null;
                                    };
                                    break;

                                case ConfigSettingType.ServerUri:
                                    this._label.Text = StringLiterals.ServerUriTextFieldPlaceholder;
                                    this._textField.Placeholder = StringLiterals.ServerUriTextFieldPlaceholder;
                                    this._textField.MaxLength = 150;
                                    this._textField.ReturnKeyType = UIReturnKeyType.Done;
                                    this._textField.AutocorrectionType = UITextAutocorrectionType.No;
                                    this._textField.AutocapitalizationType = UITextAutocapitalizationType.None;
                                    this._textField.SecureTextEntry = false;
                                    this._textField.ValidationFunction = (s) =>
                                    {
                                        if (s != null)
                                            s = s.Trim();
                                        
                                        if ((!AllowSubmitWithEmptyUriField) && String.IsNullOrEmpty(s))
                                        {
                                            _textField.TextColor = Colors.ErrorTextColor;
                                            return "Please enter a server url";
                                        }
                                        else if (ValidateUriFormat && !Uri.IsWellFormedUriString(ServerAliases.AliasToUri(s), UriKind.Absolute)) 
                                        {
                                            _textField.TextColor = Colors.ErrorTextColor; 
                                            return "Please enter a valid url";
                                        }
                                        else {
                                            _textField.TextColor = UIColor.DarkGray;
                                        }

                                        if ((!AllowSubmitWithEmptyUriField) && s.Length <= 4)
                                        {
                                            _textField.TextColor = Colors.ErrorTextColor;
                                            return "Server url must be greater than 4 characters";
                                        }
                                        else {
                                            _textField.TextColor = UIColor.DarkGray;
                                        }

                                        return null;
                                    };
                                    break;
                            }
                            this._textField.Text = setting.CurrentValue;
                            break;

                        case ConfigSettingControlType.Button:
                            this._button.Hidden = false;

                            switch (setting.Type)
                            {
                                case ConfigSettingType.LogoutButton:
                                    this._button.SetTitle(StringLiterals.LogoutButtonText, UIControlState.Normal);
                                    this._button.SetTitle(StringLiterals.LogoutButtonText, UIControlState.Disabled);
                                    this._button.SetTitleColor(Colors.ErrorTextColor, UIControlState.Normal);
                                    break;
                            }

                            break;
                    }
                });
            }

            public void Validate()
            {
                if (this._setting.ControlType == ConfigSettingControlType.TextField)
                {
                    this._setting.Valid = this._textField.Validate();

                    this._label.TextColor = this._setting.Valid ? Colors.StandardTextColor : Colors.ErrorTextColor; 
                }
            }

            protected override void HandleLayoutSubviews()
            {
                base.HandleLayoutSubviews();

                //label 
                this._label.SetFrameHeight(this.ContentView.Frame.Height);
                this._label.SetFrameWidth((this.ContentView.Frame.Width / 4) + 5); 
                this._label.SetFrameLocation(TextMargin, 0);

                //textfield 
                this._textField.SetFrameHeight(this.ContentView.Frame.Height - 10);
                this._textField.SetFrameWidth(this._label.Frame.Width * 3 - 10);
                this._textField.SetFrameX(this._label.Frame.Right + 5); 
                this._textField.CenterVerticallyInParent();

                //button 
                this._button.Frame = this.ContentView.Frame;
            }
        }

        private class ConfigSettingViewModel
        {
            //public string Label;
            public string CurrentValue;
            public ConfigSettingControlType ControlType;
            public ConfigSettingType Type;
            public bool Valid;

            public Action<string> OnValueChanged;
        }

        private enum ConfigSettingControlType
        {
            TextField,
            Button
        }

        private enum ConfigSettingType
        {
            Username,
            Password,
            ServerUri,
            LogoutButton
        }
    }
}

