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
        bool result = false;
        public ObservableRangeCollection<Item> Items { get; set; }
        public Command LoadItemsCommand { get; set; }
        public ICommand PostCommand { get; }
        public CustomEditor PostEditor { get; set; }
        public ICommand ILikeThis { get; }

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
            PostCommand = new Command(async () => await PostTask());
            ILikeThis = new Command(async (par1) => await ILikeThisTask((string)par1));

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

        async Task PostTask()
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
                if (result)
                {
                    Items.Insert(0, new Item { Id = "new", Creator = Settings.UserId, Description = newPostDescription, Likes = "0", Comments_count = "0" });

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

        public async Task<bool> TryPostAsync()
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
                EndText = "";
                start = 0;
                isNotEnd = false;
                Settings.listEnd = false;
                Items.Clear();
                var items = await DataStore.GetItemsAsync(true);
                Console.WriteLine("### osteria");
                Items.ReplaceRange(items);
                IsNotEnd = true;
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
                IsBusy = false;
            }
        }

        async Task ILikeThisTask(string par1)
        {
            Console.WriteLine(par1);
        }
    }
}
