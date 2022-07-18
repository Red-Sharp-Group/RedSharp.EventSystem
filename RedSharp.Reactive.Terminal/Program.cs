using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using RedSharp.Reactive.Sys.Abstracts;
using RedSharp.Reactive.Bindings.Helpers;

namespace RedSharp.Reactive.Terminal
{
    public class TestViewModel : ReactiveEntityBase
    {
        private bool _isChecked;

        public bool IsChecked
        {
            get => _isChecked;
            set => SetAndRaise(ref _isChecked, value);
        }
    }

    public class AnotherViewModel : ReactiveEntityBase
    {
        private ObservableCollection<TestViewModel> _model;

        public ObservableCollection<TestViewModel> Models
        {
            get => _model;
            set => SetAndRaise(ref _model, value);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var viewModel = new AnotherViewModel()
            {
                Models = new ObservableCollection<TestViewModel>()
                {
                    new TestViewModel(){ IsChecked = true},
                    new TestViewModel(){ IsChecked = true},
                    new TestViewModel(){ IsChecked = true}
                }
            };

            var expression = Expressions.Bind(viewModel)
                                        .Select(item => item.Models)
                                        .ForEachNotify<AnotherViewModel, TestViewModel, bool, ObservableCollection<TestViewModel>>
                                           (chain => chain.Select(item => item.IsChecked, (item, value) => item.IsChecked = value, nameof(TestViewModel.IsChecked)))
                                        .Unite(list => 
                                        {
                                            bool? result = null;

                                            var count = list.Count(item => item);

                                            if (count == list.Count)
                                                result = true;
                                            else if (count == 0)
                                                result = false;

                                            return result;
                                        },
                                        (list, value) => 
                                        {
                                            for (int i = 0; i < list.Count; i++)
                                                list[i] = value ?? false;
                                                
                                        })
                                        .WithCallback(value => Console.WriteLine(ToString(value)))
                                        .Commit();

            viewModel.Models[1].IsChecked = false;

            expression.EndNode.Value = false;
            expression.EndNode.Value = true;

            Console.WriteLine("Hello World!");
        }

        static String ToString(bool? value)
        {
            if (!value.HasValue)
                return "Null";
            else
                return value.Value.ToString();
        }
    }
}
