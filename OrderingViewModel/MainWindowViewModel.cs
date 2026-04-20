using OrderingModel;
using OrderingViewModel.Commands;
using OrderingViewModel.Services.Interfaces;
using OrderingViewModel.Validators;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows.Input;

namespace OrderingViewModel
{
    public class MainWindowViewModel : ObservableValidators
    {
        #region Private Fields
        private readonly IItemService _itemService;
        private readonly IOrderService _orderService;
        private Items? _selectedItem;
        private string _quantityText = string.Empty;
        private string _city = string.Empty;
        private string? _selectedState;
        private bool _isLoading;
        private bool _isSubmitting;
        private string _errorMessage = string.Empty;
        private string _confirmationMessage = string.Empty;
        #endregion

        #region Public Properties
        public ObservableCollection<Items> Items { get; } = new ObservableCollection<Items>();
        public ObservableCollection<string> States { get; } = new ObservableCollection<string>()
        {
        "KA", "MH", "TN", "DL", "GJ", "UP", "WB", "KL", "AP", "TS"
        };

       
        public Items? SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    ValidateSelectedItem();
                    _placeOrderCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string QuantityText
        {
            get => _quantityText;
            set
            {
                if (SetProperty(ref _quantityText, value))
                {
                    ValidateQuantity();
                    _placeOrderCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string City
        {
            get => _city;
            set
            {
                if (SetProperty(ref _city, value))
                {
                    ValidateCity();
                    _placeOrderCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public string? SelectedState
        {
            get => _selectedState;
            set
            {
                if (SetProperty(ref _selectedState, value))
                {
                    ValidateState();
                    _placeOrderCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            private set
            {
                if (SetProperty(ref _isLoading, value))
                {
                    OnPropertyChanged(nameof(IsBusy));
                    _placeOrderCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsSubmitting
        {
            get => _isSubmitting;
            private set
            {
                if (SetProperty(ref _isSubmitting, value))
                {
                    OnPropertyChanged(nameof(IsBusy));
                    _placeOrderCommand.RaiseCanExecuteChanged();
                }
            }
        }

        public bool IsBusy => IsLoading || IsSubmitting;

        public string ErrorMessage
        {
            get => _errorMessage;
            private set => SetProperty(ref _errorMessage, value);
        }

        public string ConfirmationMessage
        {
            get => _confirmationMessage;
            private set => SetProperty(ref _confirmationMessage, value);
        }

        #endregion

        #region ICommands
        private readonly AsyncRelayCommand _placeOrderCommand;

        public ICommand PlaceOrderCommand => _placeOrderCommand;
        public ICommand ResetCommand { get; set; }

        #endregion

        #region Constructor
        public MainWindowViewModel(IItemService itemService, IOrderService orderService)
        {
            _itemService = itemService;
            _orderService = orderService;

            _placeOrderCommand = new AsyncRelayCommand(PlaceOrderAsync, CanPlaceOrder);
            ResetCommand = new RelayCommand(Reset);
            ErrorUpdated += UpdateErrorMessage;
            _ = LoadItemsAsync();
        }
        #endregion

        #region Private Methods
        private async Task LoadItemsAsync()
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                Items.Clear();
                var items = await _itemService.GetItemsAsync();

                foreach (var item in items)
                    Items.Add(item);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to load items: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private bool CanPlaceOrder()
        {
            try
            {
                return !IsBusy
                 && !HasErrors
                 && SelectedItem is not null
                 && !string.IsNullOrWhiteSpace(QuantityText)
                 && !string.IsNullOrWhiteSpace(City)
                 && !string.IsNullOrWhiteSpace(SelectedState);
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to validate order execution conditions: {ex.Message}";
                return false;
            }
          
        }

        private async Task PlaceOrderAsync()
        {
            ValidateAll();

            if (HasErrors || SelectedItem is null)
                return;

            try
            {
                IsSubmitting = true;
                ErrorMessage = string.Empty;
                ConfirmationMessage = string.Empty;

                var quantity = int.Parse(QuantityText, CultureInfo.InvariantCulture);

                var request = new OrderRequest
                {
                    ItemId = SelectedItem.Id,
                    Quantity = quantity,
                    City = City.Trim(),
                    State = SelectedState!.Trim()
                };

                var response = await _orderService.SubmitOrderAsync(request);

                if (response.Success)
                {
                    ConfirmationMessage =
                        $"Order confirmed. Confirmation #: {response.ConfirmationNumber}";
                }
                else
                {
                    ErrorMessage = response.Message;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Order submission failed: {ex.Message}";
            }
            finally
            {
                IsSubmitting = false;
            }
        }

        private void ValidateAll()
        {
            try
            {
                ValidateSelectedItem();
                ValidateQuantity();
                ValidateCity();
                ValidateState();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to validate order: {ex.Message}";
            }
        }

        private void ValidateSelectedItem()
        {
            try
            {
                ClearErrors(nameof(SelectedItem));

                if (SelectedItem is null)
                    AddError(nameof(SelectedItem), "Please select an item.");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to validate items: {ex.Message}";
            }
        }

        private void ValidateQuantity()
        {
            try
            {
                ClearErrors(nameof(QuantityText));

                if (string.IsNullOrWhiteSpace(QuantityText))
                {
                    AddError(nameof(QuantityText), "Quantity is required.");
                    return;
                }

                if (!int.TryParse(QuantityText, out var quantity))
                {
                    AddError(nameof(QuantityText), "Quantity must be numeric.");
                    return;
                }

                if (quantity < 1 || quantity > 100)
                    AddError(nameof(QuantityText), "Quantity must be between 1 and 100.");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to validate items: {ex.Message}";
            }
        }

        private void ValidateCity()
        {
            try
            {
                ClearErrors(nameof(City));

                if (string.IsNullOrWhiteSpace(City))
                {
                    AddError(nameof(City), "City is required.");
                    return;
                }

                if (City.Trim().Length > 50)
                    AddError(nameof(City), "City must not exceed 50 characters.");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to validate city: {ex.Message}";
            }
        }

        private void ValidateState()
        {
            try
            {
                ClearErrors(nameof(SelectedState));

                if (string.IsNullOrWhiteSpace(SelectedState))
                    AddError(nameof(SelectedState), "State is required.");

                if (SelectedState?.Trim().Length > 2)
                    AddError(nameof(SelectedState), "State must not exceed 2 characters.");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Failed to validate state: {ex.Message}";
            }
        }

        private void UpdateErrorMessage(string message)
        {
            ErrorMessage = message;
        }

        private void Reset(object obj)
        {
           
            SelectedItem = null;
            QuantityText = string.Empty;
            City = string.Empty;
            SelectedState = null;
            ClearErrors(nameof(SelectedItem));
            ClearErrors(nameof(QuantityText));
            ClearErrors(nameof(City));
            ClearErrors(nameof(SelectedState));
            ErrorMessage = string.Empty;
            ConfirmationMessage = string.Empty;
        }
        #endregion
    }
}
