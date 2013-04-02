using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

// The data model defined by this file serves as a representative example of a strongly-typed
// model that supports notification when members are added, removed, or modified.  The property
// names chosen coincide with data bindings in the standard item templates.
//
// Applications may use this model as a starting point and build on it, or discard it entirely and
// replace it with something appropriate to their needs.

namespace FoodVariable.Data
{
    /// <summary>
    /// Base class for <see cref="SampleDataItem"/> and <see cref="SampleDataGroup"/> that
    /// defines properties common to both.
    /// </summary>
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SampleDataCommon : FoodVariable.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public SampleDataCommon(String uniqueId, String title, String subtitle, String imagePath, String description)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._subtitle = subtitle;
            this._description = description;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private string _subtitle = string.Empty;
        public string Subtitle
        {
            get { return this._subtitle; }
            set { this.SetProperty(ref this._subtitle, value); }
        }

        private string _description = string.Empty;
        public string Description
        {
            get { return this._description; }
            set { this.SetProperty(ref this._description, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(SampleDataCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }

    /// <summary>
    /// Generic item data model.
    /// </summary>
    public class SampleDataItem : SampleDataCommon
    {
        public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, int colSpan, int rowSpan, SampleDataGroup group)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            this._colSpan = colSpan;
            this._rowSpan = rowSpan;
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private int _rowSpan = 1;
        public int RowSpan
        {
            get { return this._rowSpan; }
            set { this.SetProperty(ref this._rowSpan, value); }
        }

        private int _colSpan = 1;
        public int ColSpan
        {
            get { return this._colSpan; }
            set { this.SetProperty(ref this._colSpan, value); }
        }


        private SampleDataGroup _group;
        public SampleDataGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    /// <summary>
    /// Generic group data model.
    /// </summary>
    public class SampleDataGroup : SampleDataCommon
    {
        public SampleDataGroup(String uniqueId, String title, String subtitle, String imagePath, String description)
            : base(uniqueId, title, subtitle, imagePath, description)
        {
            Items.CollectionChanged += ItemsCollectionChanged;
        }

        private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            // Provides a subset of the full items collection to bind to from a GroupedItemsPage
            // for two reasons: GridView will not virtualize large items collections, and it
            // improves the user experience when browsing through groups with large numbers of
            // items.
            //
            // A maximum of 12 items are displayed because it results in filled grid columns
            // whether there are 1, 2, 3, 4, or 6 rows displayed

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        if (TopItems.Count > 12)
                        {
                            TopItems.RemoveAt(12);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Move:
                    if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
                    {
                        TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
                    }
                    else if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        TopItems.Add(Items[11]);
                    }
                    else if (e.NewStartingIndex < 12)
                    {
                        TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
                        TopItems.RemoveAt(12);
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems.RemoveAt(e.OldStartingIndex);
                        if (Items.Count >= 12)
                        {
                            TopItems.Add(Items[11]);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (e.OldStartingIndex < 12)
                    {
                        TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    TopItems.Clear();
                    while (TopItems.Count < Items.Count && TopItems.Count < 12)
                    {
                        TopItems.Add(Items[TopItems.Count]);
                    }
                    break;
            }
        }

        private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> Items
        {
            get { return this._items; }
        }

        private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        public ObservableCollection<SampleDataItem> TopItems
        {
            get { return this._topItem; }
        }
    }

    /// <summary>
    /// Creates a collection of groups and items with hard-coded content.
    /// 
    /// SampleDataSource initializes with placeholder data rather than live production
    /// data so that sample data is provided at both design-time and run-time.
    /// </summary>
    public sealed class SampleDataSource
    {
        private static SampleDataSource _sampleDataSource = new SampleDataSource();

        private ObservableCollection<SampleDataGroup> _allGroups = new ObservableCollection<SampleDataGroup>();
        public ObservableCollection<SampleDataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<SampleDataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _sampleDataSource.AllGroups;
        }

        public static SampleDataGroup GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SampleDataItem GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }



        public SampleDataSource()
        {
            String ITEM_CONTENT = String.Format("Item Content: {0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}\n\n{0}",
                        "In Pakistan and India every woman knows that bride is incomplete without applying beautiful and stunning mehndi designs on her hands, feet and arms. These days, applying mehndi becomes popular fashion. Every year, numerous mehndi designs are coming for women and young girls. In this post, we are presenting latest and exclusive mehndi designs 2013 for women. Women and girls can apply these mehndi designs on their hands, arms and feet. All mehndi designs 2013 are simply stunning and magnificent. These mehndi designs 2013 include different types of designs like floral designs, peacock designs and many more. If we talk about these mehndi designs then some mehndi designs are extremely beautiful but difficult. So women can apply them with the help of professional mehndi artist. On the other hand, some of them are simple then even girls can easily apply them without taking any help.");

            var group1 = new SampleDataGroup("Group-1",
                 "Get Ex-Boy Friend Back",
                 "Group Subtitle: 1",
                 "Assets/DarkGray.png",
                 "Group Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs. In this style we can use different styles of mehndi like Black mehndi is used as outline, fillings with the normal henna mehndi. We can also include sparkles as a final coating to make the henna design more attractive.");

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item1",
                 "Tip 01",
                 "Tip 01",
                 "Assets/HubPage/HubpageImage2.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "The Right Steps for Love It can be incredibly difficult to think about trying to get ex-boyfriend back, especially if you feel like he is ignoring you. This is not only frustrating, it can make you feel like you are completely undesirable as well. Thankfully, there are some things you can do to get this relationship back on track. The first thing you need to do is determine why he is ignoring you. If the relationship ended badly, then perhaps you both need space. If you are always trying to contact him, it will only serve to drive him further away. If you truly want to get your ex-boyfriend back, you're going to need to use some important and time-tested measures. It's time to get back to what made you desirable to him in the first place! Think back to the point where you first got together. What did you look like? What did you act like? The chances are very good that the time the two of you spent together was great in your mind -- but something must've changed or gone wrong to cause the breakup. As you think about these things, you're going to need to begin a journey of self-discovery and getting right with yourself once again. That doesn't mean you're going to change yourself in order to conform to his standards. Rather, you're going to become a whole person who can stand on her own without having a man by her side. This might sound hokey to you, and next to impossible, but this is absolutely essential to do if you want to have any hope of getting him back in your arms again.\n\nAfter you have taken these steps, some time will have passed. He might have even started to miss you. At any rate, you will appear more mature and collected since you have given him space. \n\nNow's the time where you can casually start to talk to him once again. As you start talking more, you might hang out as friends and see a movie or eat dinner together. From this point on, you can treat things as if you are getting together for the very first time. The timing on this is very critical. Trying to get your ex-boyfriend back can blow up in your face if you rush things or take too long. It might sound nerve-wracking that the advice for how to get him to stop ignoring you is to cease communication! \n\nIt's truly the best way as it gives him the space he needs so you can finally get together again when the time is right. My advice to you is research as much as you can about this topic in order to win him back to you.",
                 35,
                 35,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item2",
                 "Tip 02",
                 "Tip 02",
                 "Assets/HubPage/HubpageImage3.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "Love Detox to Win Him Back Again If all you can think about is how to get my ex-boyfriend back! it can be very difficult to stay focused on the things you need to do in your life. It doesn't matter whether you are in school or hold a full-time job, thinking about him all the time can really effect your life negatively. That's not to say there is no hope for winning him back into your arms. That's actually very far from the truth! Many women have successfully won their boyfriends back by taking the right steps. However, this process needs to start with self-exploration and self-improvement. If you are neglecting your school, home, or work life, it's time to take steps to correct that. The best way to get started is to think about the task at hand. Consider what you're truly passionate about in the first place. Perhaps you haven't felt any passion for anything else since you started dating your boyfriend. You can tap into that passion again, and get a renewed sense of energy just by realizing that you can! This will allow you to focus more easily on the task at hand. Now, even if you feel good about doing this at first, you might lose some steam as you start to miss him more and more. It's time to put those pictures of him away and to cease all contact -- just for now. \n\nJust like you might detox from an addiction, you need to detox from your ex-boyfriend! This might be a painful experience right now, but you will come out on the other side better for it. \n\nDid you know that many boyfriends break up with her girlfriends because they need some space? It may not be a matter of how much they do or do not love you. If they feel smothered it can seem like they simply have no other way out. Now that you are giving him this space and getting your focus back on other things in your life, your chances of reconnecting soon are much better. Men like women who are not needy or clingy. Constantly calling and pining away for him is not the way to win him back! With time, you will be able to stop thinking about trying trying to get my ex-boyfriend back, because you will have already gotten him!\n\nThis can only happen when you are completely ready and feeling good as an individual. It means dragging yourself out of bed and doing what you need to do until the time is right.",
                 35,
                 35,
                 group1));

            group1.Items.Add(new SampleDataItem("Small-Group-1-Item3",
                 "Tip 03",
                 "Tip 03",
                 "Assets/HubPage/HubpageImage4.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "Getting Your Ex Back and Getting Rid of Grudges Do you think about getting your ex back all the time? If so, there are probably a lot of questions on your mind. One of the first things women wonder when they are considering getting back together with their boyfriend is if it is going to work this time. It makes sense to be a little hesitant -- especially if things ended badly the first time. Before you take any more steps, you need to evaluate the relationship you used to have. If there was absolutely any abuse at all (verbal or physical), you need to take actions to end the relationship and all contact now. Even if you feel like the love is still there, it is not worth feeling the pain. In addition to abuse, relationships can just be a bad match. The feelings of love might be there, or just the feeling of familiarity that draws people together. If you're getting back together just because you are used to being together, or people expect you to be, that is not a good reason! You need to get back together because you are truly in love with one another, and because it is the best thing for both of you.\n\nIf you have determined that getting your ex back is the best course of action, it's time to breathe a sigh of relief. The chances are very good that you were nervous while you were broken up. Thoughts of your own self-worth were probably very low, and you may have thought that you would never escape from the dark feelings.\n\nIt's also time to let go of any guilt you feel over the breakup. If it was a specific action that was your fault, and that has been forgiven by him, it's time to forgive yourself as well. The same holds true if it was he who did something wrong. If you have pledged to forgive him for whatever it was, it's time to actually do so! Your relationship cannot grow if there are grudges and hurt feelings all around. This does not mean that things will automatically be back to normal or that old issues will never creep up. It's just important at this early stage in the game to take whatever steps you can take to prevent the problem from becoming so large that it causes you to break up all over again. Some couples will need to seek counseling, while others will be fine to relish in the fact that they are back together, and can let those old feelings go.\n\nWith that being said -- congratulations! Getting your ex back is absolutely the best feeling in the entire world. If you are not to this point, but only dream of it, do whatever you can to read about and take the necessary steps to win him back into your arms for good.",
                 35,
                 35,
                 group1));

            group1.Items.Add(new SampleDataItem("Big-Group-1-Item4",
                 "Tip 04",
                 "Tip 04",
                 "Assets/HubPage/HubpageImage5.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "How NOT Talking to Him Can Make Him Want You Getting your ex-boyfriend back can seem like a far-off dream if you don't think there is any hope. This can lead to the question of how you get your ex-boyfriend to wish he had YOU back, instead of it being a single-sided feeling. Actually, you might be surprised at how he really does feel. While you are wishing to get your boyfriend back, he might be wishing to have you back into his arms as well! Now, don't get your hopes up, as he may need some more time to let these feelings build again. The breakup might be too fresh in his mind -- especially if it was your fault.\n\nThe best way to get him to want you back is to give him some space. This sounds incredibly counterintuitive, and is even a little bit ironic. \n\nDo you feel like he is pushing you away, or that you are constantly bothering him? This happens to many girlfriends. It can even turn into an obsession for you to have to try and talk to him all the time. You think that by wearing him down he will finally want you back again. This will only serve to push him away so far that you lose all hope of ever getting back together -- so don't do it! When you give him space he is able to think about what he liked about the relationship. Absence truly does make the heart grow fonder for couples who are meant to be together. He is not able to miss you if you are constantly bothering him! Unfortunately, It can be difficult to stop this kind of behavior, especially if you feel desperate. One thing you might do is ask one of your friends if they can be your sponsor of sorts. Contacting your ex-boyfriend can certainly become an addiction. Whenever you feel the desire to contact him come on, you can call this friend so they can talk you out of it and help you keep your eye on the prize. After enough time has passed, the chances are very good that he will miss you and be ready to get back together. Of course, there are other steps you need to keep in mind. The good news is that there are some relationship guides on the market that can help you discover exactly how to win him back for good. This information is probably priceless to you, since you know that he is truly the love of your life.",
                 69,
                 70,
                 group1));

            group1.Items.Add(new SampleDataItem("Landscape-Group-1-Item5",
                 "Tip 05",
                 "Tip 05",
                 "Assets/HubPage/HubpageImage6.png",
                 "Item Description: Arabic mehndi designs are very beautiful and complete the most part of the hands and legs.",
                 "What to Do If You Have to See Him All the Time If you're trying to get your ex-boyfriend back, it can be incredibly frustrating to see him around all the time. Some people are very lucky in that their paths never cross until they're ready to reconnect in the future. However, if you attend school together or you work together, this can be near impossible. It can also make the process of winning him back a lot more difficult. The standard advice that I give as a relationship expert is to cease all contact for now. If you HAVE to see one another this can't happen, and there is no use beating yourself up about it. Still, taking a break from communicating really does work in healing wounds and preparing yourself for your eventual reconnection. If you have no possible alternative but to see him, do not despair. What you are going to do is more difficult than what others have to do, but it can still be done! The first step is being able to concentrate on your life as it would have been if you had not been together. Do not act like you can't possibly function with him around. While your heart might be palpitating, you need to keep a cool and calm exterior.\n\nYou need to present yourself as a self-sufficient and happy woman. Flaunt what you've got as an individual! Think about what attracted him to you in the first place, and do everything you can to get that girl back again. \n\nTalk with other coworkers or peers and develop strong friendships so you have other people to depend on. This is more crucial than ever, as you probably became fully dependent on your ex boyfriend since you constantly saw each other.\n\nYou also need to do something extra special for yourself. Perhaps it is tackling a new project that you wouldn't have had time to do otherwise. Showcase your talents and your drive to succeed, and be the person that you are as an individual.\n\nYour ex-boyfriend will probably be very impressed by this turn of events. Instead of constantly fielding text messages and awkward situations that he just can't wait to get away from, you will be giving him the space he needs. At the same time, he will see you at your absolute best. If those sparks used to be there, the chances are very good that they will flare up again.\n\nYou really can get your ex-boyfriend back even if you have to see each other all the time! While you have to take some extra steps that others do not have to take, your chances of giving your lover back are still very good if you follow the right steps.",
                 69,
                 35,
                 group1));

            

            this.AllGroups.Add(group1);

            var group2 = new SampleDataGroup("Group-2",
                "Get Ex-Girl Friend Back",
                "Group Subtitle: 2",
                "Assets/DarkGray.png",
                "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group2.Items.Add(new SampleDataItem("Big-Group-2-Item1",
                "Tip 01",
                "Tip 01",
                "Assets/HubPage/HubpageImage7.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "How to Get Back Your Ex Girlfriend If you've been reading the relationship advice on how to get back your ex-girlfriend, you have no doubt come across the no contact phase. This is something that many relationship gurus recommend because it is incredibly helpful when you are trying to win back a lost love. If you are not familiar with that phrase, here's a brief summary. Basically, it is incredibly damaging to any relationship you might have in the future if you're constantly calling, texting, or chatting online with your ex-girlfriend. The chances are very good that she is simply getting frustrated with you and those ill feelings are just stacking on top of one another. Doing this can ruin your hopes for good! After you have waited long enough (usually a month or more), it is time to reconnect. One good way to do this is to get together on a casual date just to see one another again. You can present this as a way to just chat about things that have gone on in your life. Hopefully, enough time has passed that your ex-girlfriend is willing to do this.\n\nYou should have also followed the advice of bettering yourself in between the time you were dating and made contact again. Your hope is that once your ex-girlfriend sees you and talks to you, she will remember the things she always loved about you. From that point on, you can slowly move back into subtle things you used to -- especially through subtle touches, and Kino. Doing this is an art, and far too many men rush into things. It's in your best interest to educate yourself as much as possible and read everything you can get your hands on on how to get back your ex-girlfriend.  Some of the information can be overwhelming. The best thing to do is consider how it relates to your personal situation. Above all, don't let impatience get in the way. If you are impatient during this process, she might slip through your fingers forever.\n\nListen, there is nothing better than getting to hold her in your arms once again. Planning a simple get-together can help get those feelings going. Subtle touches and good conversation can give you the feeling that you're able to start over with one another once again. This is just what you need!",
                69,
                70,
                group2));

            group2.Items.Add(new SampleDataItem("Landscape-Group-2-Item2",
                "Tip 02",
                "Tip 02",
                "Assets/HubPage/HubpageImage8.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "What You Need to do to Get Ex-Wife Back Trying to get your ex-wife back can be one of the most difficult and frustrating things you ever have to do. After spending so much of your time together, it can feel like your other half is gone! This is made even more difficult by the fact that your bond was legal at one point, and now that has been broken. Still, there is no reason to despair because you CAN win her back into your arms for good. Of course, you have to realize that it is a case-by-case basis. What works for one couple may not work for another. Some advice is universal, however! The first thing you need to do is stop the bad habits! This means ending the constant contact until you are able to take the right steps to repair the relationship. It can be difficult to not talk to the person you love most in this world, but it is necessary. While you're taking some time away, you're definitely going to need to get right with yourself. Think of things you used to enjoy doing before you were a couple. Do things you forgot you missed! You should also take steps to become healthier physically and mentally. If you need to see someone professionally, now is the time to do so. Those steps will be crucial in helping you to get ex-wife back -- no matter how desperate you are right now. You probably feel like you are in the depths of despair and will never get out! After you have taken the time to become healthier and happier as an individual, you will feel 10 times better. You will then be at a point where you are ready to get back together with the love of your life. After you have done those things, it is time to plan your reconnection. This is where things get particularly tricky. Before you jump right in, you need to be sure that what caused you to break up in the first place has been corrected, or can be corrected. If it is something that was your fault, consider whether holding onto this is more important than winning back the love of your life. If you're truly in love, the chances are very good that it's not. Please do everything you can to further your education as you try to get your ex-wife back. There are some amazing resources that have been written by experts who have tutored thousands upon thousands of men in getting back their ex-girlfriends and wives. Put these techniques to use for yourself and you should have great results. Nothing is better than rekindling a lost love!",
                69,
                35,
                group2));

            group2.Items.Add(new SampleDataItem("Medium-Group-2-Item3",
                "Tip 03",
                "Tip 03",
                "Assets/HubPage/HubpageImage9.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Get Your Ex-Girlfriend Back -- Simple Advice You Need Now How are you going to get your ex-girlfriend back? If you're like many men, you have absolutely no idea what to do at this point. Reading these simple tips can help give you a head start in winning back your lost love forever. Don't be that guy -- you know what I'm talking about! Don't be that guy that calls her all the time and sends her heartbreaking text messages that only serve to annoy her. This is definitely not the way to get her back. Those who seem desperate end up without the love of their life. Do things you like doing -- when you're in a relationship, it can be easy to forget to do the things YOU like to do. Now that you are single, it's not time to wallow in self-pity alone at home. Obviously this is easier said than done, but you need to do it. Go out with friends and maybe even start dating again. These dates don't need to be serious -- just a way to pass the time, have fun, and feel attractive again.\n\nDon't seem desperate -- a huge mistake that many men make when they're trying to get their ex-girlfriend back is begging out of desperation. This is not attractive to a woman and can ruin your chances before you know it. Being confident in yourself and in the relationship is going to be essential if you want to win her back. Know how to reconnect -- top relationship experts have developed excellent strategies for reconnecting with your ex. This involves techniques that many men are unaware of. You can reconnect too early, and can you wait until it is too late. The best thing for you to do is plan a time to get together when there is little pressure, and some of the negative feelings from the break up have worn off. Then, you can get some of those old feelings back to the way they used to be. The timing on this is crucial -- this can't be said enough!\n\nThis is just the tip of the iceberg when it comes to trying to get your ex-girlfriend back. Absorbing as much information as possible before you attempt to do this can be just what you need to be able to start loving again. If you know that your ex-girlfriend is the love of your life, you absolutely need to do everything you can to get back in her arms.",
                41,
                41,
                group2));

            group2.Items.Add(new SampleDataItem("Medium-Group-2-Item4",
                "Tip 04",
                "Tip 04",
                "Assets/HubPage/HubpageImage09.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Help! Get My Ex-Girlfriend Back You're probably saying over and over to yourself, I need to get my ex-girlfriend back! if you feel like you've lost her forever. The hard part is figuring out how you tell your ex-girlfriend that you want to get back together with her in the first place. Or, perhaps you tell her this all the time and you are met with a lot of resistance. Understanding how to overcome these obstacles is the only way you're going to be able to rekindle your relationship. The first thing you need to do is determine what went wrong in the first place. This is different with every single relationship, so it's something you'll need to discover on your own. If possible, you should take some time to discuss this with your ex-girlfriend -- but do not put any pressure on her at this point in time. Simply present it as a catalyst for your own self growth for relationships in the future. Now that you know what went wrong, it is not the time to dwell on things. You do want to fix them, but you need to take some time away from the situation. You should consider this the no contact phase. This should last for at least a period of one month. This will give you time to better yourself and prepare yourself to be in a relationship again. It will also give her time to start forgetting the things that went wrong, and start remembering the things she misses about you!When the time is right, you're going to tell her that you want to get back together with her. Don't jump into this too fast -- this is where many men make their mistake. After you have taken some time off, it's not going to be helpful for you to call her up and say you want to get back together out of the blue! It is far better to plan a reconnection where you meet as friends first. That way you can get to know each other again without any pressure.\n\nOf course, that's not all there is to it! There are certain things you need to do at this point that will help smooth the process, since she moved along more quickly. Uncovering this advice is how you're going to get the answer to your question of, how do I get my ex girlfriend back? You need to take these steps to be successful.",
                41,
                41,
                group2));

            
            this.AllGroups.Add(group2);


            

            var group3 = new SampleDataGroup("Group-3",
               "General",
               "Group Subtitle: 2",
               "Assets/DarkGray.png",
               "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");

            group3.Items.Add(new SampleDataItem("Big-Group-3-Item1",
                "Tip 01",
                "Tip 01",
                "Assets/HubPage/HubpageImage10.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Get Ex Back -- Even if They've Moved On Trying to get an ex back can be the most difficult thing you have ever done in your life. Being in a hard relationship is certainly no walk in the park, but it can be a little easier than feeling like you've lost the love of your life for good. Some evaluation of your relationship is essential to determine if you should move forward from this point with the same person. First of all, there is a difference between love and feeling a sense of familiarity with the person. Far too often we fall into certain patterns that feel comfortable -- such as being with the same person for an extended period of time. Some people actually consider this familiarity as a reason to breakup. They can't stand to be with the the same person day after day, when there are other people out there. Still others wouldn't have things any other way. It is this incompatibility that causes many couples to split in the first place. No matter which situation you are in, you need to figure out if the two of you are truly in love. Unfortunately, there are cases of one-sided love where you feel like you have met the person you want to spend the rest of your life with, but they don't feel the same way. That's why you need to do a true evaluation of the situation. Consider whether or not the breakup was difficult for your ex partner. \n\nSome people are quick to point to the presence of a new relationship as evidence that the other person has completely moved on. This is not always the case. Sometimes people think they want something new and different, but find that YOU were what they wanted all along. They may try to fill the void with a new person, but it doesn't compete with the real thing! Unfortunately, knowing that their ex is dating someone new causes many people to give up altogether -- effectively ending things with the love of their life without giving it a fair fight.\n\nLuckily, you are not going to let that happen. You can get your ex back if you know the two of you are meant to be together. Don't worry -- that doesn't mean you are going to have to become stalker crazy! There are certain steps you can take to get right with yourself, and feel better as an individual before you even consider joining as a couple once again.\n\nThe good news is that after you have done this evaluation of yourself and your relationship, the chances are very good that you can get your ex back forever. It's time to drag yourself out of the self-pity, and take steps that will help remedy the situation.",
                69,
                70,
                group3));

            group3.Items.Add(new SampleDataItem("Landscape-Group-3-Item2",
                "Tip 02",
                "Tip 02",
                "Assets/HubPage/HubpageImage11.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Get My Ex Back -- Turn off the Noise and Get Some Good Advice Saying to yourself, how can I get my ex back? is probably driving you crazy. You may be asking for advice from friends and family members only to receive confusing information from all sides. The truth of the matter is that while they may have been in your shoes before, the chances are good that they were not successful in getting their ex back, which can be discouraging. That's why it's important to make sure you're getting your information from the best possible place. It's not enough to take advice from anyone you run across. This can be damaging to your relationship and cause you to lose the love of your life forever. \n\nFor example, have you ever had a friend tell you to just call him? This is very damaging, and can lead to frustration on both sides. While your ex needs their space, you have people telling you to fight for them by constantly contacting them. Filtering out where you receive advice from will help you in many ways. For one thing, it will not be so confusing as to who to listen to. You'll also finally have time to focus on self-improvement and preparing yourself to be in a relationship again, rather than on doing what Aunt Sally said versus your older brother.\n\nThankfully, there are some amazing guides on the market that were written by relationship experts. It is essential that anything you read be written by someone who has been in your shoes. That way you know that they know how you need the information presented, and what kind of information should be there. It is also helpful to see that other people have had success with the advice the guru has given them. Finding reviews on products makes choosing the right guide easier than ever. The Internet makes it so that you can read comments and recommendations on products from people all over the world. Unfortunately, there is not much on the market out on the topic of getting your ex back that comes in print form. The good news is that there are downloadable books that are instantly given to you once you purchase them. The ease of getting this information should definitely be a plus for you. Time is truly of the essence and you don't want to make any mistakes that could possibly damage your relationship forever. Constantly thinking about trying to get my ex back can be confusing enough, so it's a great thing to have expert advice to turn to.",
                69,
                35,
                group3));

            group3.Items.Add(new SampleDataItem("Medium-Group-3-Item3",
                "Tip 03",
                "Tip 03",
                "Assets/HubPage/HubpageImage12.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "Secrets of Getting an Ex Back Into Your Arms -- For Good This Time By this point, you may have read several different things that deal with getting an ex back. You may have even asked friends or family members for advice. Unfortunately, the chances are very good that you're feeling very confused right now, and don't know where to turn. The good news is that you can breathe a sigh of relief! It IS VERY possible to win your ex back for good. It doesn't matter how things ended or how hopeless everything might seem right now, you can turn this into a positive, workable relationship once again. I'm going to caution you that this is going to take some time. This isn't good news for you to hear, because you feel that time is of the essence (and you're right!) The difficulty here is that if you push things too fast you can erase your chances. That brings us to the first secret...let your ex go; for now! This is hard to think about, but it's the right thing.\n\nIf you're used to holding on, you are probably smothering them. Breaking up can be hard for both parties, but it is made even more difficult if you talk to one another all the time. All this does is open up negative feelings. You may also be fighting all the time. This does nothing for you as a couple, and it does nothing for you as individuals.\n\nAnother secret to getting an ex back is slyly planning a period of reconnection. After you have given one another enough space, you'll probably feel like you can get together casually just to catch up. This is very non-threatening, and is a great window of opportunity for you. Once again, it's essential not to rush things! Take it slow, and make sure that you don't bring up getting back together until the timing is absolutely right.\n\nThere are some very subtle things you can do to open those thoughts again. Touching in a certain way can bring back a flood of positive memories for both of you. It can almost feel like no time has passed, and you have been in each other's arms all along. Subtle suggestions are key as you open their mind more and more to the fact that you belong together. If you do this right, they will feel like it was their idea to get back together all along!",
                41,
                41,
                group3));
            group3.Items.Add(new SampleDataItem("Medium-Group-3-Item4",
               "Tip 04",
               "Tip 04",
               "Assets/HubPage/HubpageImage13.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "Getting Back Together -- How to Deal With Old Feelings If you have broken up with your ex, but things are beginning to feel more normal now, you probably have some questions about getting back together. Many couples who get back together sort of feel like they are having their first date all over again. Things may be awkward, especially if there are unanswered questions on the table. While it is important to sort out past problems in the relationship, you probably don't want to discuss anything too heavy when you first start dating again. Consider it a clean slate for now, even though these important issues are not gone forever. When the timing is right, you can discuss these past issues so they do not rise up again in an ugly fashion. In fact, waiting too long can mean that these same issues ruin your relationship again! That's why it is an important balancing act. On the other hand, discussing things too early can wreck any chance you have of gaining new, positive memories and moving on from the past. Waiting too long can make these things fester under the surface and catch you both by surprise later on.\n\nYou might be pleasantly surprised that by getting back together, you really have made some great changes in your own life. Your partner may have made some changes too. This may completely eliminate some of the problems you had in the past. However, you should realize that it does take a while for some people to change, and some people never will. It might take some doing on your part to simply get over problems you have with the issue, or find some other outlet for them. This truly is an individual case. Another important part of getting back together is learning to do fun things again. Hopefully you have gone through old mementos and discovered amazing times you had together when you first started dating. Perhaps you should do some of the same things to encourage those good memories once again. It's also a great idea to create new wonderful memories with one another! The more you do together, the happier you will be -- as long as you're not focusing on petty things and you feel incredibly blessed that you are back together once again.\n\nI don't want to paint an entirely rosy picture of getting back together. You might go through some trial and error as you adjust to your new relationship. Still, there is no reason not to believe that love can overcome all, and that your relationship will be amazing once again.",
               41,
               41,
               group3));

            this.AllGroups.Add(group3);


         



            var group4 = new SampleDataGroup("Group-4",
               "Useful Tricks for all",
               "Group Subtitle: 2",
               "Assets/DarkGray.png",
               "Group Description: Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vivamus tempor scelerisque lorem in vehicula. Aliquam tincidunt, lacus ut sagittis tristique, turpis massa volutpat augue, eu rutrum ligula ante a ante");
            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item1",
               "Trick 01",
               "Trick 01",
               "Assets/HubPage/HubpageImage14.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "Get Your Ex Back -- Two Things You Need to Do Now Do you feel like there is no hope for you to get your ex back? If so, it's time to take some action to get those negative thoughts out of your head. The fact is that thousands, perhaps even millions, of couples have gotten back together after they have broken up. Many of these relationships have ended in extremely happy marriages! It's time to win back the love of YOUR life for good, as soon as possible. This going to involve some exercises to help you through. The first thing you should do is write down what you think caused the breakup. The chances are very good that you know exactly what this was. It might have been your fault, or it may have been your partner's fault. In most cases, it was a combination of faults that caused the breakup. The point of this is not to pin the blame on just one person. Instead, it is to get things out in the open in your own mind so you can move past them.\n\nUp until this point, I have spoken only about writing things down for your own benefit. That is because you are probably not ready to speak to your ex quite yet. Even if you feel that you are ready, you may not be truly ready. This could damage your relationship for good if you try to force things on them too early. After you have thought about these very negative aspects, it is time to think about what was good when you were together. This exercise will help you determine if it is the right path to get back together with your ex. Make a list of all the memories you have together. These can be surprising memories, wonderful memories, or incredibly loving memories. Whatever you feel are the best moments of your relationship are what you should write down.\n\nHopefully, this exercise will help to bring a smile on your face. You've probably been frowning far too much lately, and it is time to feel good again. By bringing forth these happy memories, you are solidifying in your mind that getting back together is a good idea. These memories are what you have to fight for since you want to create new ones with the person you have lost. Trying to get your ex back can be nerve-wracking, but by doing these two simple exercises you will be increasing your chances of success.",
               41,
               41,
               group4));

            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item2",
                "Trick 02",
                "Trick 02",
                "Assets/HubPage/HubpageImage15.png",
                "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
                "How to Get Over An Ex -- Or Not! If you have decided that it is not a good idea for you and your ex to get back together, it can be very frustrating. Perhaps you're not even to the decision-making point, but you fear the worst. Discovering how to get over your ex is an important step in the healing process, so you can start to feel good about yourself as an individual once again. The first thing you need to do is come to terms with things that went wrong. If they were your fault, consider what you might do differently in future relationships. If it is something your previous partner did wrong, you now know what character traits to look out for when it comes to choosing a partner later on.\n\nIn addition to thinking about what went wrong, you definitely have to think about what went right as well. If you're having difficulty getting over your love, the chances are very good that there were aspects you really did enjoy about that relationship. This might be the way that person looked at you, the things you did together, or the in-depth conversations you used to have. Consider the good qualities that person has so you can focus on a person who has similar traits in the future. Of course, getting over your ex isn't always that simple. Even if you try to heal your wounds and move on, your heart may be in a different place. If that is the case, and you really have tried to move on, I urge you to attempt to get back together with your ex. If some time has passed you might be pleasantly surprised to find that they are ready to get back together with you as well!\n\nIn that case, you absolutely need to follow the best relationship advice you can find. This might mean investing in this advice so you can put it into action right away. The good news is that there are some excellent products on the market that can help you with this no matter which stage you're in right now. Even though many people break up, not many people know the secrets to getting back together. Unfortunately, many of these tactics really are secrets. People are simply not skilled enough in relationships to know what they need to do and what they shouldn't do. By studying material from relationship experts, you can avoid mistakes and do the right things so you can get your ex back. Sometimes, learning how to get over it includes getting the person of your dreams back!",
                41,
                41,
                group4));
            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item3",
               "Trick 03",
               "Trick 03",
               "Assets/HubPage/HubpageImage16.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "Win Back Your Ex -- It's Possible With The Right Timing Are you trying to win back your ex? If so, you're definitely not alone. Thousands of people break up all the time, and many of them feel like there is no hope of getting back together. The good news for you is that you are reading the best advice on winning back a lost love! The first thing you need to do is stop all contact right now. Consistently keeping in contact will only serve to drive you the two of you further apart. You are probably feeling very emotional right now, and you both may even be saying things you don't mean. This is especially true if they are the person that ended the relationship. You should give them some time to themselves, because they may not even be ready to talk to you. \n\nBy waiting this out, you will be giving them the opportunity to reflect on the relationship and whether or not it was the right thing. You might find that they ARE ready to get back together with you, without you having to do anything additional. Unfortunately, this is not always the case, so you will need to take additional steps. Part of this is figuring out who you are as an individual. If you spend every single waking moment thinking of your ex, it is not a healthy thing! You need to get out there, hang out with friends, and possibly even start dating again. It doesn't matter whether you are interested in starting another long-term relationship or not; the only important thing is that you start to feel valued and have some fun again. \n\nYou should also really work on your health. This means eating healthy foods and exercising. Healthy foods will help you to feel less fatigue (something you're probably feeling a lot of right now), and help you maintain a slimmer figure. Exercise is known to release endorphins that can improve your mood right away. This can be a lifesaver if you are feeling depressed or uneasy about the entire situation. When the timing is right, it is a good idea to connect with them again. This is a very crucial step when you want to win back your ex, because you really can get the timing off. Connecting too early will mean those hurt feelings are still there, and your ex is will not be ready to get back together. Waiting too long can mean all the feelings of love are forgotten -- or they have moved on. Since you don't want this to happen, absorb every piece of information you can get your hands on to make everything better.",
               41,
               41,
               group4));
            group4.Items.Add(new SampleDataItem("Medium-Group-4-Item4",
               "Trick 04",
               "Trick 04",
               "Assets/HubPage/HubpageImage17.png",
               "Item Description: Pellentesque porta, mauris quis interdum vehicula, urna sapien ultrices velit, nec venenatis dui odio in augue. Cras posuere, enim a cursus convallis, neque turpis malesuada erat, ut adipiscing neque tortor ac erat.",
               "How to Get Back with Your Ex Girlfriend If you're like many men who have just broken up with someone they love, you may have had the conversation that ended in let's just be friends. This is very unfortunate and, even if you agreed to at the time, it may not be what you are feeling in your heart. Knowing what to say and how to move on from here is going to be key.  The first thing you need to decide is if you want to get back with your ex-girlfriend. If you know in your heart that it was wrong to break up, this may be the right path for you. However, if the relationship caused both of you more harm than good in your lives, then it may be time to let go. Knowing what to do in that case is crucial. If you are letting that person go, then you may want to take some time apart. When the time is right, you can reconnect as friends, or even as acquaintances if that becomes more fitting. There is no reason that you have to be best friends forever if the two of you have personalities that just don't mesh. On the other hand, you might be in the boat where you still desperately want to get back together with your ex-girlfriend. Don't despair -- there is hope for you as well! Actually, the first part of the advice that you need to follow is the same as for those who are going to remain friends with their ex-girlfriend. You need to take some time apart and give her some space for now. While you're giving her space, you need to work on your life mentally, physically, and emotionally. By bettering yourself, you'll be in a great place to start this relationship up again. When you are ready to get back together with her, you may want to do so as friends. This is especially true if you have had the let's just be friends conversation.  You don't have to leave it at that!\n\nThere are things you can do once you have reconnected to spark those flames again. In fact, some of these things are so sly (yet, completely ethical and above board) that she'll think it was her idea to get back together with you. No matter who decides what, it's a great feeling once you know how to get back with your ex-girlfriend.",
               41,
               41,
               group4));
            this.AllGroups.Add(group4);



        }
    }
}
