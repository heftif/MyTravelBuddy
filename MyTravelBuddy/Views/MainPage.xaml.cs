namespace MyTravelBuddy;

public partial class MainPage : ContentPage
{
    public MainPage(MainViewModel vm)
    {
        InitializeComponent();
        // it is necessary to inject the vm and set it as binding context to
        // connect the view to the viewmodel!
        BindingContext = vm;
 
    }

}


