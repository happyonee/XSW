﻿using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xamarin.Forms;

namespace StritWalk
{
    public class ItemDetailViewModel : BaseViewModel
    {

        string result = string.Empty;
        public Item Item { get; set; }
        public ObservableRangeCollection<CommentsItem> CommentsItems { get; set; }
        public Command PostCommentCommand { get; }
        public CustomListView listView { get; set; }
        int quantity = 1;
        public int Quantity
        {
            get { return quantity; }
            set { SetProperty(ref quantity, value); }
        }


        public ItemDetailViewModel(object item = null)
        {
            Item = item as Item;
            Title = "Comments";
            CommentsItems = new ObservableRangeCollection<CommentsItem>();            
            PostCommentCommand = new Command(async (par1) => await PostCommentTask((object)par1));
        }

        public async Task LoadComments()
        {
            IsLoading = true;
            try
            {
                if (Item.Comments != null)
                {
                    var items = await DataStore.GetComments(Item.Id);
                    CommentsItems.ReplaceRange(items);                    
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("xxx " + ex);
            }
            IsLoading = false;
        }

        async Task PostCommentTask(object par1)
        {
            try
            {                
                result = await DataStore.PostComment(Item.Id, (string)par1);
            }
            finally
            {                
                if (!string.IsNullOrWhiteSpace(result))
                {
                    var item = new CommentsItem { User_name = Settings.UserId, Comment = (string)par1 };
                    CommentsItems.Add(item);
                    if (Device.RuntimePlatform == Device.iOS)
                        listView.ScrollTo(item, ScrollToPosition.End, true);
                    Item.Comments_count = CommentsItems.Count.ToString();                    
                    var jitem = new JObject();
                    jitem["user_name"] = item.User_name;
                    jitem["comment"] = item.Comment;
                    Item.Comments.Insert(0, jitem);
                    Item.VisibleComments = true;
                    Item.ViewComments = "";
                    //MessagingCenter.Send(this, "NewComment", Item);
                }                
            }
        }
    }
}
