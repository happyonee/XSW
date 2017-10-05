﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Threading;

using Xamarin.Forms;

namespace StritWalk
{
    public class ItemsViewModel : BaseViewModel
    {
        public int start = 0;
        string result = string.Empty;
        public ObservableRangeCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public ICommand PostCommand { get; }
        public CustomEditor PostEditor { get; set; }
        public ICommand ILikeThis { get; }
        public User me;
        public ICommand ICommentThis { get; }

        string newPostDescription = string.Empty;
        public string NewPostDescription
        {
            get { return newPostDescription; }
            set { SetProperty(ref newPostDescription, value); }
        }

        string endText = string.Empty;
        public string EndText
        {
            get { return endText; }
            set { SetProperty(ref endText, value); }
        }

        Color postPlaceholder = Color.Black;
        public Color PostPlaceholder
        {
            get { return postPlaceholder; }
            set { SetProperty(ref postPlaceholder, value); }
        }

        bool isNotEnd = false;
        public bool IsNotEnd { get { return isNotEnd; } set { SetProperty(ref isNotEnd, value); } }

        public ItemsViewModel()
        {
            Title = "Seahorse";
            Items = new ObservableRangeCollection<Item>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            PostCommand = new Command(async (par1) => await PostTask((object)par1));
            ILikeThis = new Command(async (par1) => await ILikeThisTask((object)par1));
            ICommentThis = new Command(async (par1) => await ICommentThisTask((object)par1));
            me = new User();
            //Task.Run(() => GetMyUser());

            MessagingCenter.Subscribe<NewItemPage, Item>(this, "AddItem", async (obj, item) =>
            {
                var _item = item as Item;
                Items.Add(_item);
                await DataStore.AddItemAsync(_item);
            });

            MessagingCenter.Subscribe<CloudDataStore, bool>(this, "NotEnd", (sender, arg) =>
            {
                IsNotEnd = arg;
            });
        }

        void insertItem(Item item)
        {
            try
            {
				Console.WriteLine(Items[0].Description);
				Items.Insert(0, item);    
            }
            catch(Exception ex)
            {
                Console.WriteLine("### EX ### " + ex);
            }

        }

        async Task PostTask(object par1)
        {
            try
            {
                // Post method
                IsLoading = true;
                result = await TryPostAsync();
            }
            finally
            {
                IsLoading = false;
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var newitem = new Item { Id = result, Nuovo = true, Creator = Settings.UserId, Description = newPostDescription, Likes = "0", Comments_count = "0", Distanza = "0", Liked_me = "0" };
                    ObservableRangeCollection<Item> Items2 = new ObservableRangeCollection<Item>();
                    //Items.Insert(0, newitem);
                    insertItem(newitem);

                    Settings.Num_posts += 1;
                    me.Num_posts += 1;
                    PostsN = new FormattedString
                    {
                        Spans =
                        {
                            new Span { Text = "Posts" + "\n", FontSize=11.0F, ForegroundColor=Color.FromHex("#000000")},
                            new Span { Text = me.Num_posts.ToString(), FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=Color.FromHex("#000000") }
                        }
                    };

                    string text = "Posted. Do you want to post something else?";
                    PostEditor.Placeholder = text;

                    if (Device.iOS == Device.RuntimePlatform)
                    {
                        PostEditor.Text = text;
                    }
                    else
                    {
                        PostEditor.Text = "";
                    }
                }
                else
                {
                    string text = "Do you want to post something?";
                    PostEditor.Placeholder = text;
                }
                IsPosting = false;
                PostEditor.Unfocus();
            }
        }

        public async Task<string> TryPostAsync()
        {
            return await DataStore.Post("", "", "", "", "", newPostDescription);
        }

        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                await Task.Run(() => GetMyUser());
                EndText = "";
                start = 0;
                IsNotEnd = false;
                Settings.listEnd = false;
                //Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                Items.ReplaceRange(items);
                IsNotEnd = true;
                EndText = "";
                //Items.Insert(0, new Item());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                MessagingCenter.Send(new MessagingCenterAlert
                {
                    Title = "Error",
                    Message = "Unable to load items.",
                    Cancel = "OK"
                }, "message");
            }
            finally
            {
                //IsNotEnd = false;
                IsBusy = false;
            }
        }

        async Task ILikeThisTask(object par1)
        {
            if (IsBusy)
                return;

            IsBusy = true;
            Item item = par1 as Item;
            string action = "addLikePost";
            if (item.Liked_me == "#2b98f0")
                action = "removeLikePost";
            var res = await DataStore.ILikeThis((string)item.Id, action);
            if (res == 2)
            {
                var num = Int32.Parse(item.LikesNum);
                num -= 1;
                item.Likes = num.ToString();
                item.Liked_me = "0";
            }
            else if (res == 0)
            {
                var num = Int32.Parse(item.LikesNum);
                num += 1;
                item.Likes = num.ToString();
                item.Liked_me = "1";
            }
            IsBusy = false;
        }

        async Task GetMyUser()
        {
            me = await DataStore.GetMyUser(me);
            PostsN = new FormattedString
            {
                Spans =
                    {
                        new Span { Text = "Posts" + "\n", FontSize=11.0F, ForegroundColor=Color.FromHex("#000000")},
                        new Span { Text = me.Num_posts.ToString(), FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=Color.FromHex("#000000") }
                    }
            };
            LikesN = new FormattedString
            {
                Spans =
                    {
                        new Span { Text = "Liked" + "\n", FontSize=11.0F, ForegroundColor=Color.FromHex("#000000")},
                        new Span { Text = me.Num_likes.ToString(), FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=Color.FromHex("#000000") }
                    }
            };
            FriendsN = new FormattedString
            {
                Spans =
                    {
                        new Span { Text = "Followers" + "\n", FontSize=11.0F, ForegroundColor=Color.FromHex("#000000")},
                        new Span { Text = me.Num_friends.ToString(), FontSize=11.0F, FontAttributes=FontAttributes.Bold, ForegroundColor=Color.FromHex("#000000") }
                    }
            };
        }

        async Task ICommentThisTask(object par1)
        {
            CustomTabbedPage page = Application.Current.MainPage as CustomTabbedPage;
            page.TabBarHidden = true;
            await Navigation.PushAsync(new ItemDetailPage(new ItemDetailViewModel(par1)));
            //await Application.Current.MainPage.Navigation.PushModalAsync(new NavigationPage(new ItemDetailPage(new ItemDetailViewModel(par1))));
        }
    }
}
