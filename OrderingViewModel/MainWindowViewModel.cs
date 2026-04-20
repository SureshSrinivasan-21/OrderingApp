using OrderingModel;
using OrderingViewModel.Commands;
using OrderingViewModel.Services.Interfaces;
using OrderingViewModel.Validators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace OrderingViewModel
{
    public class MainWindowViewModel : ObservableValidators
    {
        private readonly IItemService _itemService;
        private readonly IOrderService _orderService;
        private readonly AsyncRelayCommand _placeOrderCommand;

        private Items? _selectedItem;
        private string _quantityText = string.Empty;
        private string _city = string.Empty;
        private string? _selectedState;
        private bool _isLoading;
        private bool _isSubmitting;
        private string _errorMessage = string.Empty;
        private string _confirmationMessage = string.Empty;

        public ObservableCollection<Items> Items { get; } = new();
        public ObservableCollection<string> States { get; } = new()
    {
        "KA", "MH", "TN", "DL", "GJ", "UP", "WB", "KL", "AP", "TS"
    };

        public ICommand PlaceOrderCommand => _placeOrderCommand;

        public MainWindowViewModel(IItemService itemService, IOrderService orderService) 
        {
            _itemService = itemService;
            _orderService = orderService;

            _placeOrderCommand = new AsyncRelayCommand(PlaceOrderAsync, CanPlaceOrder);

            _ = LoadItemsAsync();
        }
         
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
            return !IsBusy
                   && !HasErrors
                   && SelectedItem is not null
                   && !string.IsNullOrWhiteSpace(QuantityText)
                   && !string.IsNullOrWhiteSpace(City)
                   && !string.IsNullOrWhiteSpace(SelectedState);
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
            ValidateSelectedItem();
            ValidateQuantity();
            ValidateCity();
            ValidateState();
        }

        private void ValidateSelectedItem()
        {
            ClearErrors(nameof(SelectedItem));

            if (SelectedItem is null)
                AddError(nameof(SelectedItem), "Please select an item.");
        }

        private void ValidateQuantity()
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

        private void ValidateCity()
        {
            ClearErrors(nameof(City));

            if (string.IsNullOrWhiteSpace(City))
            {
                AddError(nameof(City), "City is required.");
                return;
            }

            if (City.Length > 50)
                AddError(nameof(City), "City must not exceed 50 characters.");
        }

        private void ValidateState()
        {
            ClearErrors(nameof(SelectedState));

            if (string.IsNullOrWhiteSpace(SelectedState))
                AddError(nameof(SelectedState), "State is required.");
        }
    }
}
