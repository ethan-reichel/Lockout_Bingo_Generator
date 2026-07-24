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
			public string Title { get; set; }
			public List<Board> Boards { get; set; } 
			public List<Goal> Goals { get; set; }
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
		//Functionality for adding an objective or category
		private void AddGameButton_Click(object sender, RoutedEventArgs e)
        {
			InputJSON();
		}
        
        private void GameInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
				InputJSON();
            }
		}


		//Script for the Input function
		private void InputJSON()
        {
            string InputText = Input_Box.Text;
            if (!string.IsNullOrWhiteSpace(InputText))
            {

				_data.Games.Add(new Game {Title = InputText });
				SaveData();
				Input_Box.Clear();
            }
		}

		/*private int GetNextGameId()
		{
			if(_data.Games.Count == 0)
				return 0;

			return _data.Games.Max(g => g.Id) + 1;
		}*/

		private void OpenTab(object sender, RoutedEventArgs e) {
			MenuItem clickedButton = sender as MenuItem;
			Game clickedGame = clickedButton.Tag as Game;

			Grid newGrid = TabTemplate;
			string xaml = XamlWriter.Save(TabTemplate);
			Grid content = (Grid)XamlReader.Parse(xaml);
			
			TabItem newTab = new TabItem();
			newTab.Header = clickedGame.Title;
			newTab.MinWidth = 100;
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
				GamesList.SelectedItem = null;
			}
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