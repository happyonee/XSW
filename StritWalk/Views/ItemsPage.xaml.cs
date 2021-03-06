﻿using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Diagnostics;


[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace StritWalk
{
    public partial class ItemsPage : ContentPage
    {
        public ItemsViewModel viewModel;
        //IList<Item> items;
        public IDataStore<Item> DataStore => DependencyService.Get<IDataStore<Item>>();
        public Command LoadMoreCommand { get; }

        public ItemsPage()
        {
            InitializeComponent();

            BindingContext = viewModel = new ItemsViewModel();
            viewModel.Navigation = Navigation; //Navigation
            viewModel.PostEditor = PostEditor;
            LoadMoreCommand = new Command(async () => await LoadMoreItems(null));
            //App.AppNav = Navigation;

            MessagingCenter.Subscribe<ItemsViewModel, bool>(this, "IsAudioPost", (obj, arg) =>
            {
                if (arg) PlayPostButton.TextColor = (Color)Application.Current.Resources["Testo4"];
                else PlayPostButton.TextColor = (Color)Application.Current.Resources["Testo5"];
            });
            MessagingCenter.Subscribe<ItemsViewModel, bool>(this, "IsPosting", (obj, arg) =>
            {
                if (!arg) PostButton.TextColor = (Color)Application.Current.Resources["Testo5"];
                else PostButton.TextColor = (Color)Application.Current.Resources["App1"];
            });
            MessagingCenter.Subscribe<ItemDetailPage, bool>(this, "ExitComments", (obj, arg) =>
            {
                ItemsListView.ScrollTo(App.currentItem, ScrollToPosition.Start, false);
            });

            var firstTemplate = new DataTemplate(() =>
            {
                var w = Content.Width;
                var postLabel = new ClickableLabel { Margin = new Thickness(20, 0, 20, 0) };
                postLabel.Clicked += async (sender, e) =>
                {
                    await Navigation.PushAsync(new PostPage(viewModel.Items, e.PItem));
                    //await App.AppNav.PushAsync(new PostPage(viewModel.Items, e.PItem));
                };
                postLabel.SetBinding(ClickableLabel.ItemProperty, ".");
                postLabel.SetBinding(Label.FormattedTextProperty, "Post");
                var separatorLine = new BoxView { HeightRequest = 1, BackgroundColor = (Color)Application.Current.Resources["Sfondo2"], Margin = new Thickness(0, 0, 0, 0) };
                var numbersLabel = new Label { Margin = new Thickness(20, 0, 20, 0), TextColor = (Color)Application.Current.Resources["Testo3"], FontSize = 12 };
                numbersLabel.SetBinding(Label.TextProperty, "NumberOfLikes");
                var separatorLine3 = new BoxView { HeightRequest = 1, BackgroundColor = (Color)Application.Current.Resources["Sfondo2"], Margin = new Thickness(20, 0, 20, 0) };
                var playButton = new Button() { TextColor = (Color)Application.Current.Resources["App1"], FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Color.Transparent, BorderColor = Color.Transparent };
                playButton.Text = "Play";
                //playButton.SetBinding(IsEnabledProperty, "AudioExist");
                playButton.Command = viewModel.IPlayThis;
                playButton.SetBinding(Button.CommandParameterProperty, ".");
                var likeButton = new Button() { Text = "Like", FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Color.Transparent, BorderColor = Color.Transparent };
                likeButton.SetBinding(Button.TextColorProperty, "Liked_me_color");
                likeButton.Command = viewModel.ILikeThis;
                likeButton.SetBinding(Button.CommandParameterProperty, ".");
                var commentsButton = new Button() { Text = "Comment", TextColor = (Color)Application.Current.Resources["Testo4"], FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Color.Transparent, BorderColor = Color.Transparent };
                commentsButton.Command = viewModel.ICommentThis;
                commentsButton.SetBinding(Button.CommandParameterProperty, ".");
                var separatorLine2 = new BoxView { HeightRequest = 1, BackgroundColor = (Color)Application.Current.Resources["Sfondo2"], Margin = new Thickness(0, 0, 0, 0) };
                var whiteSeparator = new BoxView { BackgroundColor = (Color)Application.Current.Resources["Sfondo2"], HeightRequest = 10, Margin = new Thickness(0, 0, 0, 0) };

                //absolute layout
                var layout = new AbsoluteLayout();
                AbsoluteLayout.SetLayoutBounds(postLabel, new Rectangle(0, 10, w, 60));
                AbsoluteLayout.SetLayoutBounds(separatorLine, new Rectangle(0, 80, w, 1));
                AbsoluteLayout.SetLayoutBounds(numbersLabel, new Rectangle(0, 91, w, 28));
                AbsoluteLayout.SetLayoutBounds(separatorLine3, new Rectangle(0, 114, w, 1));
                AbsoluteLayout.SetLayoutBounds(playButton, new Rectangle(0, 109, w / 3, 48));
                AbsoluteLayout.SetLayoutBounds(likeButton, new Rectangle(w / 3, 109, w / 3, 48));
                AbsoluteLayout.SetLayoutBounds(commentsButton, new Rectangle((w / 3) * 2, 109, w / 3, 48));
                AbsoluteLayout.SetLayoutBounds(separatorLine2, new Rectangle(0, 152, w, 1));
                AbsoluteLayout.SetLayoutBounds(whiteSeparator, new Rectangle(0, 152, w, 10));

                layout.Children.Add(postLabel);
                layout.Children.Add(separatorLine);
                layout.Children.Add(numbersLabel);
                layout.Children.Add(separatorLine3);
                layout.Children.Add(playButton);
                layout.Children.Add(likeButton);
                layout.Children.Add(commentsButton);
                layout.Children.Add(separatorLine2);
                layout.Children.Add(whiteSeparator);

                CustomViewCell cell = new CustomViewCell();
                cell.View = layout;

                return cell;
            });

            var secondTemplate = new DataTemplate(() =>
            {
                var w = Content.Width;
                var postLabel = new ClickableLabel { Margin = new Thickness(20, 0, 20, 0) };
                postLabel.Clicked += async (sender, e) =>
                {
                    await Navigation.PushAsync(new PostPage(viewModel.Items, e.PItem));
                };
                postLabel.SetBinding(ClickableLabel.ItemProperty, ".");
                postLabel.SetBinding(Label.FormattedTextProperty, "Post");
                var separatorLine = new BoxView { HeightRequest = 1, BackgroundColor = (Color)Application.Current.Resources["Sfondo2"], Margin = new Thickness(0, 0, 0, 0) };
                var numbersLabel = new Label { Margin = new Thickness(20, 0, 20, 0), TextColor = (Color)Application.Current.Resources["Testo3"], FontSize = 12 };
                numbersLabel.SetBinding(Label.TextProperty, "NumberOfLikes");
                var separatorLine3 = new BoxView { HeightRequest = 1, BackgroundColor = (Color)Application.Current.Resources["Sfondo2"], Margin = new Thickness(20, 0, 20, 0) };
                var playButton = new Button() { TextColor = (Color)Application.Current.Resources["App1"], FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Color.Transparent, BorderColor = Color.Transparent };
                playButton.Text = "Play";
                playButton.SetBinding(IsEnabledProperty, "AudioExist");
                playButton.Command = viewModel.IPlayThis;
                playButton.SetBinding(Button.CommandParameterProperty, ".");
                var likeButton = new Button() { Text = "Like", FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Color.Transparent, BorderColor = Color.Transparent };
                likeButton.SetBinding(Button.TextColorProperty, "Liked_me_color");
                likeButton.Command = viewModel.ILikeThis;
                likeButton.SetBinding(Button.CommandParameterProperty, ".");
                var commentsButton = new Button() { Text = "Comment", TextColor = (Color)Application.Current.Resources["Testo4"], FontSize = 12, FontAttributes = FontAttributes.Bold, Margin = new Thickness(0, 0, 0, 0), BackgroundColor = Color.Transparent, BorderColor = Color.Transparent };
                commentsButton.Command = viewModel.ICommentThis;
                commentsButton.SetBinding(Button.CommandParameterProperty, ".");
                var separatorLine2 = new BoxView { HeightRequest = 1, BackgroundColor = (Color)Application.Current.Resources["Sfondo2"], Margin = new Thickness(0, 0, 0, 0) };
                var commentsLabel = new CommentsLabel { Margin = new Thickness(20, 0, 20, 0), Text = "", TextColor = (Color)Application.Current.Resources["Testo3"], FontSize = 13 };
                commentsLabel.SetBinding(Label.FormattedTextProperty, "ViewComments");
                commentsLabel.SetBinding(CommentsLabel.ItemProperty, ".");
                commentsLabel.Clicked += (sender, e) =>
                {
                    viewModel.ICommentThis.Execute(e.PItem);
                };
                //var tapGestureRecognizer = new TapGestureRecognizer();
                //tapGestureRecognizer.Tapped += (s, e) => {
                //    Console.WriteLine(string.Format("obj: {0}, e: {1}", s.ToString(), e.ToString()));
                //}; 
                //tapGestureRecognizer.Command = viewModel.ICommentThis;
                //tapGestureRecognizer.SetBinding(TapGestureRecognizer.CommandParameterProperty, ".");
                //commentsLabel.GestureRecognizers.Add(tapGestureRecognizer);
                var whiteSeparator = new BoxView { BackgroundColor = (Color)Application.Current.Resources["Sfondo2"], HeightRequest = 10, Margin = new Thickness(0, 0, 0, 0) };

                //absolute layout
                var layout = new AbsoluteLayout();
                AbsoluteLayout.SetLayoutBounds(postLabel, new Rectangle(0, 10, w, 60));
                AbsoluteLayout.SetLayoutBounds(separatorLine, new Rectangle(0, 80, w, 1));
                AbsoluteLayout.SetLayoutBounds(numbersLabel, new Rectangle(0, 91, w, 28));
                AbsoluteLayout.SetLayoutBounds(separatorLine3, new Rectangle(0, 114, w, 1));
                AbsoluteLayout.SetLayoutBounds(playButton, new Rectangle(0, 109, w / 3, 48));
                AbsoluteLayout.SetLayoutBounds(likeButton, new Rectangle(w / 3, 109, w / 3, 48));
                AbsoluteLayout.SetLayoutBounds(commentsButton, new Rectangle((w / 3) * 2, 109, w / 3, 48));
                AbsoluteLayout.SetLayoutBounds(separatorLine2, new Rectangle(0, 152, w, 1));
                AbsoluteLayout.SetLayoutBounds(commentsLabel, new Rectangle(0, 157, w, 55));
                AbsoluteLayout.SetLayoutBounds(whiteSeparator, new Rectangle(0, 212, w, 10));

                layout.Children.Add(postLabel);
                layout.Children.Add(separatorLine);
                layout.Children.Add(numbersLabel);
                layout.Children.Add(separatorLine3);
                layout.Children.Add(playButton);
                layout.Children.Add(likeButton);
                layout.Children.Add(commentsButton);
                layout.Children.Add(separatorLine2);
                layout.Children.Add(commentsLabel);
                layout.Children.Add(whiteSeparator);

                CustomViewCell cell = new CustomViewCell();
                cell.View = layout;
                return cell;
            });

            ItemsListView.ItemTemplate = new CustomDataTemplateSelector
            {
                //FirstTemplate = new DataTemplate(typeof(TemplateOneViewCell)),
                //SecondTemplate = new DataTemplate(typeof(TemplateTwoViewCell))
                FirstTemplate = firstTemplate,
                SecondTemplate = secondTemplate
            };

            ItemsListView.ItemSelectedCustomEvent += async (object o, SelectedItemChangedEventArgs e) =>
            {
                Item item = e.SelectedItem as Item;
                await Navigation.PushAsync(new PostPage(viewModel.Items, item));
                ItemsListView.SelectedItem = null;
            };

        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnItemSelected(object sender, SelectedItemChangedEventArgs args)
        {
            //var item = args.SelectedItem as Item;
            //if (item == null)
            //return;

            //await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(item)));

            //await CrossMediaManager.Current.Play("http://www.hackweb.it/api/uploads/music/" + item.Audio);             

            ItemsListView.SelectedItem = null;
        }

        private void OnCellTapped(object sender, EventArgs args)
        {
            ItemsListView.SelectedItem = null;
        }

        private async void OnFocused(object sender, EventArgs args)
        {
            if (viewModel.NewPostDescription == PostEditor.Placeholder)
            {
                viewModel.NewPostDescription = string.Empty;
            }

            await Task.Delay(250);
            viewModel.IsPosting = true;
            PostButton.TextColor = (Color)Application.Current.Resources["App1"];
        }

        private void OnCheckTest(object sender, EventArgs args)
        {
            if (String.IsNullOrEmpty(PostEditor.Text) || String.IsNullOrWhiteSpace(PostEditor.Text))
            {
                string text = "Do you want to post something?";
                PostEditor.Placeholder = text;
                viewModel.IsPosting = false;
                PostButton.TextColor = (Color)Application.Current.Resources["Testo5"];
            }
        }

        private async void OnItemTapped(object sender, ItemTappedEventArgs args)
        {
            Console.WriteLine("@@@@@ itemtapped");
            Item item = args.Item as Item;
            await Navigation.PushAsync(new PostPage(viewModel.Items, item));
            ItemsListView.SelectedItem = null;
        }



        void OnReachBottom(object sender, ItemVisibilityEventArgs args)
        {

            //if (PostEditor.IsFocused && viewModel.IsPosting)
            //    PostEditor.Unfocus();

            if (viewModel.Items[viewModel.Items.Count - 1] == args.Item && !Settings.listEnd)
            {
                if (!viewModel.IsBusy)
                    Task.Run(() => LoadMoreItems(args.Item));
            }
            else
            {
                viewModel.EndText = "The End";
            }

        }

        async Task LoadMoreItems(object x)
        {
            try
            {
                viewModel.start += 20;
                viewModel.IsBusy = false;
                var items = await DataStore.GetItemsAsync(true, viewModel.start);
                viewModel.Items.AddRange(items);
                //foreach (var item in items)
                //{
                //    viewModel.Items.Add(item);
                //}
            }
            catch (Exception ex)
            {
                Debug.WriteLine("# EXCEPTION # \n there was a fuckin exception: " + ex);
            }
            finally
            {
                viewModel.IsBusy = false;
            }
        }

        async void AddItem_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new NewItemPage(viewModel.Items));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (viewModel.Items.Count == 0)
                viewModel.LoadItemsCommand.Execute(null);
        }


        void Handle_Tapped(object sender, System.EventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
