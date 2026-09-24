using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

//JSON additions
using System.IO;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using System.Windows.Threading;

//This program is built with the Bingosync website in mind, and is not built
//for Lockout.live
namespace Lockout_Bingo_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
		//Initialize the window
		private StorageData _data;
		private readonly string _filePath = Path.Combine(
			AppContext.BaseDirectory, "..", "..", "..", "BingoStorage.json");

		public MainWindow()
        {
            InitializeComponent();
			LoadData();
			GamesList.ItemsSource = _data.Games;

			//_selectedBorder = ObjectivesBorder;
			//_selectedBorder.BorderBrush = new SolidColorBrush(Colors.Yellow);
			//_selectedBorder.BorderThickness = new Thickness(4);
			//_selectedLabel = ObjectivesLabel;
			//_selectedLabel.FontWeight = FontWeights.Bold;
			//_selectedLabel.Foreground = new SolidColorBrush(Colors.Cyan);

		}
		//Initialize the JSON compatability
		private void LoadData()
		{
			if (File.Exists(_filePath))
			{
				string json = File.ReadAllText(_filePath);
				_data = JsonConvert.DeserializeObject<StorageData>(json);
			}
			else
			{
				_data = new StorageData { Games = new ObservableCollection<Game>() };
			}
		}

		private void SaveData()
		{
			if (_data == null)
			{
				MessageBox.Show("Data is null");
				return;
			}
			string json = JsonConvert.SerializeObject(_data, Formatting.Indented);
			File.WriteAllText(_filePath, json);


		}
		//The objects that will be used to insert data to the JSON file

		public class Board
		{ 
			public int Id {  get; set; }
			public string Name { get; set; }
		}
		public class Override
		{
			public List<int> Board_Id { get; set; }
			public List<object> Range { get; set; }
		}
		public class Token
		{
			public int Id { get; set; }
			public string Name { get; set; }
			public string Type { get; set; }
			public List<object> DefaultRange { get; set; }
			public List<Override> Overrides { get; set; }
		}
		public class Goal
		{
			public int Id { get; set; }
			public string Goal_Text { get; set; }
			public List<int> Boards { get; set; }
			public List<Token> Tokens { get; set; }
		}
		public class Game
		{
			//Variables
			public string Title { get; set; }
			public List<Board> Boards { get; set; } 
			public List<Goal> Goals { get; set; }

			//Classes

			//public class Board
			//{
			//	public int Id { get; set; }
			//	public string Name { get; set; }
			//	public int GoalCount { get; set; }	
			//}

			//public class Goal{
			//	public int Id { get; set; }
			//	public string Goal_Text { get; set; }
			//	public List<int> Boards { get; set; }
			//	public List<Token> Tokens { get; set; }

			//	//Classes
			//	public class Token
			//	{
			//		public int Id { get; set; }
			//		public string Name { get; set; }
			//		public string Type { get; set; }
			//		public List<object> DefaultRange { get; set; }
			//		public List<Override> Overrides { get; set; }

			//		//Classes
			//		public class Override
			//		{
			//			public List<int> Board_Id { get; set; }
			//			public List<object> Range { get; set; }
			//		}
			//	}
			//}
		}
		public class StorageData
		{
			public ObservableCollection<Game> Games { get; set; }
		}

		//Fetch and Push JSON
		public void JSONsend(){//sending the data to the JSON file
			
		}
		public void JSONrecieve(){//getting the data from the JSON file
			
		}
		
		
		//=============== Functions for adding a new game to the list ===============
		private void AddGameButton_Click(object sender, RoutedEventArgs e)
        {
			AddGame();
		}
        
        private void GameInput_KeyDown(object sender, KeyEventArgs e)
        {
			//Make sure we are in the Games List Tab
			if (e.Key == Key.Enter)
			{
				TabItem selectedTab = MainTabControl.SelectedItem as TabItem;
				string tabHeader = selectedTab.Header.ToString();
				if (tabHeader == "Games List")
					AddGame();
			}
		}
		private void AddGame()
        {
            string InputText = Game_Input.Text;
            if (!string.IsNullOrWhiteSpace(InputText))
            {
				if (IsUniqueGame(InputText))
				{
					_data.Games.Add(new Game { Title = InputText });
					SaveData();
					Game_Input.Clear();
				}else{
					ShowWarning("Game Already Exists", Game_Input);
				}
            }
		}
		public bool IsUniqueGame(string newInput) {
			foreach (Game game in _data.Games)
				if (game.Title == newInput)
					return false;
			return true;
		}
		//=============== Functions for adding a new game to the list ===============
		
		
		//=========== Functions for adding a new board to the list ===========


		public bool IsUniqueBoard(string newInput, string gameTitle){
			Game selectedGame = _data.Games.FirstOrDefault(g => g.Title == gameTitle);
			if (selectedGame.Boards.Any(b => b.Name == newInput))
				return false;
			return true;
		}
		
		
		//======= Tooltip Warning ========
		public void ShowWarning(string message, UIElement element){ 
			ToolTip tooltip = new ToolTip{
				Content = message,
				Background = new SolidColorBrush(Colors.White),
				Foreground = new SolidColorBrush(Colors.Red),
				BorderBrush = new SolidColorBrush(Colors.DarkRed),
				Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom,
				PlacementTarget = element,
				IsOpen = true
			};
			element.SetValue(ToolTipProperty, tooltip);

			DispatcherTimer timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(3);
			timer.Tick += (s, e) =>
			{
				tooltip.IsOpen = false;
				timer.Stop();
			};
			timer.Start();

		}
		//======= Tooltip Warning ========


		private void OpenTab(object sender, RoutedEventArgs e) {
			MenuItem clickedButton = sender as MenuItem;
			Game clickedGame = clickedButton.Tag as Game;

			Grid newGrid = TabTemplate;
			string xaml = XamlWriter.Save(TabTemplate);
			Grid content = (Grid)XamlReader.Parse(xaml);
			
			TabItem newTab = new TabItem();
			newTab.Header = clickedGame.Title;
			newTab.MinWidth = 100;
			newTab.Focusable = true;
			newTab.MouseLeftButtonDown += (s, e) => { Window_Click(s,e);};
			newTab.Background = new SolidColorBrush(Colors.Transparent);
			newTab.Content = content;
			if(!IsTabOpen(clickedGame.Title))
				MainTabControl.Items.Add(newTab);
		}

		private void DeleteGame_Click(object sender, RoutedEventArgs e){
			MenuItem clickedButton = sender as MenuItem;
			Game clickedGame = clickedButton.Tag as Game;

			MessageBoxResult result = MessageBox.Show(
				$"Are you sure you want to delete '{clickedGame.Title}'?",
				"Confirm Delete",
				MessageBoxButton.YesNo,
				MessageBoxImage.Warning
			);
			if (result == MessageBoxResult.Yes)
			{
				_data.Games.Remove(clickedGame);
				SaveData();
				RemoveTabByName(clickedGame.Title);
			}
		}
		private bool IsTabOpen(string tabName)
		{
			foreach (TabItem tab in MainTabControl.Items)
			{
				if (tab.Header.ToString() == tabName)
				{
					return true;
				}
			}
			return false;
		}
		private void RemoveTabByName(string tabName)
		{
			TabItem tabToRemove = null;
			foreach (TabItem tab in MainTabControl.Items) {
				if(tab.Header.ToString() == tabName){
					tabToRemove = tab;
					break;
				}
			}
			if (tabToRemove != null)
			{
				MainTabControl.Items.Remove(tabToRemove);
			}
		}
		private void Window_Click(object sender, MouseButtonEventArgs e){
			
			MenuItem clickedItem = sender as MenuItem;
			if (clickedItem == null)
			{
			Keyboard.ClearFocus();
				GamesList.SelectedItem = null;
				if (sender is UIElement element)
					element.Focus();
				//Game_Input.focus();
			}
		}

        private void Input_Box2_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        //     private void Input_Select(object sender, MouseButtonEventArgs e){
        //Grid selectedGrid = sender as Grid;
        //Border selectedBorder = selectedGrid.Children[1] as Border;
        //Label selectedLabel = selectedGrid.Children[0] as Label;
        //Label inputLabel = Input_Label;

        //if(_selectedBorder != null){
        //	_selectedBorder.BorderBrush = new SolidColorBrush(Colors.Gray);
        //	_selectedBorder.BorderThickness = new Thickness(2);
        //	_selectedLabel.FontWeight = FontWeights.Normal;
        //	_selectedLabel.Foreground = new SolidColorBrush(Colors.White);
        //}
        //_selectedBorder = selectedBorder;

        //_selectedBorder.BorderBrush = new SolidColorBrush(Colors.Yellow);
        //_selectedBorder.BorderThickness = new Thickness(4);
        //_selectedLabel = selectedLabel;
        //_selectedLabel.Foreground = new SolidColorBrush(Colors.Cyan);
        //if(_selectedLabel.Name == "ObjectivesLabel"){
        //	inputLabel.Content = "Enter Objective:";
        //} else if( _selectedLabel.Name == "CategoriesLabel"){
        //	inputLabel.Content = "Enter Category:";
        //}
        //     }
    }
}