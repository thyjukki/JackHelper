using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

namespace JackHelper
{
    public class TreeViewModel : INotifyPropertyChanged
    {
        public TreeViewModel()
        {
            _parents = new List<TreeViewModel>();
        }
        public TreeViewModel(string name) :this()
        {
            Name = name;
            Children = new List<TreeViewModel>();
        }

        #region Properties

        public string Name { get; private set; }
        public List<TreeViewModel> Children { get; private set; }
        public bool IsInitiallySelected { get; private set; }
        public Computer AttachedComp;

        bool? _isChecked = false;
        List<TreeViewModel> _parents;

        private bool isExpanded;
        public bool IsExpanded
        {
            get { return isExpanded; }
            set
            {
                isExpanded = value;
                OnPropertyChanged("IsExpanded");
            }
        }

        #region IsChecked

        public bool? IsChecked
        {
            get { return _isChecked; }
            set { SetIsChecked(value, true, true); }
        }

        void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (value == _isChecked) return;

            _isChecked = value;

            if (updateChildren && _isChecked.HasValue) Children.ForEach(c => c.SetIsChecked(_isChecked, true, false));

            foreach (var _parent in _parents)
            {
                if (updateParent && _parent != null) _parent.VerifyCheckedState();
            }

            NotifyPropertyChanged("IsChecked");
        }

        void VerifyCheckedState()
        {
            bool? state = null;

            for (int i = 0; i < Children.Count; ++i)
            {
                bool? current = Children[i].IsChecked;
                if (i == 0)
                {
                    state = current;
                }
                else if (state != current)
                {
                    state = null;
                    break;
                }
            }

            SetIsChecked(state, false, true);
        }

        #endregion

        #endregion

        public void Initialize()
        {
            foreach (TreeViewModel child in Children)
            {

                child._parents.Add (this);
                child.Initialize();
            }
        }

        public static List<TreeViewModel> SetTree(string topLevelName)
        {
            List<TreeViewModel> treeView = new List<TreeViewModel>();
            TreeViewModel tv = new TreeViewModel(topLevelName);

            treeView.Add(tv);

            //Perform recursive method to build treeview 

            #region Test Data
            //Doing this below for this example, you should do it dynamically 
            //***************************************************
            TreeViewModel tvChild4 = new TreeViewModel("Child4");

            tv.Children.Add(new TreeViewModel("Child1"));
            tv.Children.Add(new TreeViewModel("Child2"));
            tv.Children.Add(new TreeViewModel("Child3"));
            tv.Children.Add(tvChild4);
            tv.Children.Add(new TreeViewModel("Child5"));

            TreeViewModel grtGrdChild2 = (new TreeViewModel("GrandChild4-2"));

            tvChild4.Children.Add(new TreeViewModel("GrandChild4-1"));
            tvChild4.Children.Add(grtGrdChild2);
            tvChild4.Children.Add(new TreeViewModel("GrandChild4-3"));

            grtGrdChild2.Children.Add(new TreeViewModel("GreatGrandChild4-2-1"));
            //***************************************************
            #endregion

            tv.Initialize();

            return treeView;
        }

        public void FindTree(ref List<Computer> collector)
        {
            Computer computer = this.AttachedComp;
            if (computer != null)
            {
                if (IsChecked.HasValue)
                {
                    if (IsChecked.Value)
                    {
                        if (!collector.Contains(computer))
                        {
                            collector.Add(computer);
                        }
                    }

                    return;
                }
            }

            foreach (var item in Children)
            {
                item.FindTree(ref collector);
            }
        }

        public List<Computer> GetTree()
        {
            List<Computer> computers = new List<Computer>();
            FindTree(ref computers);

            List<Group> groups = new List<Group>();
            foreach (var comp in computers)
            {
                if (comp.GetType() == typeof(Group))
                {
                    groups.Add((Group)comp);
                }
            }

            List<Computer> compsInGroups = new List<Computer>();
            foreach (var group in groups)
            {
                computers.RemoveAll(x => group.Computers.Contains(x));
            }

            return computers;
        }

        #region INotifyPropertyChanged Members

        void NotifyPropertyChanged(string info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
        /// <summary>
        /// Raises the PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The name of the property that has changed.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
