using System;
using DropBoxSync.iOS;
using MonoTouch.Foundation;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace MonkeyBox
{
	public class DropboxDatabase
	{
		public event EventHandler MonkeysUpdated;

		static DropboxDatabase shared;

		public static DropboxDatabase Shared {
			get {
				if (shared == null)
					shared = new DropboxDatabase ();
				return shared;
			}
		}

		DBDatastore store;
		public DropboxDatabase ()
		{
			Monkeys = new Monkey[0];
		}

		public void Init()
		{
			if (store != null)
				return;
			DBError error;
			store = DBDatastore.OpenDefaultStoreForAccount (DBAccountManager.SharedManager.LinkedAccount, out error);
			store.AddObserver (store, () => {
				LoadData();
			});
		}
		public Dictionary<string,DBRecord> records = new Dictionary<string, DBRecord> ();
		public Dictionary<string,Monkey> monkeyDictionary = new Dictionary<string, Monkey> ();
		public Task LoadData ()
		{
			var task = Task.Factory.StartNew (() => {
				var table = store.GetTable ("monkeys");
				DBError error;
				var results = table.Query (new NSDictionary (), out error);

				//This deletes old data
//				foreach(var result in results)
//				{
//					result.DeleteRecord();
//				}
//				store.Sync(null);
//				populateMonkeys();
//				results = table.Query (new NSDictionary (), out error);
				///
				records = results.ToDictionary(x=> x.Fields["Name"].ToString(),x=> x);
				if (results.Length == 0) {
					populateMonkeys ();
					return;
				}
				foreach(var result in results)
				{
					var name = result.Fields["Name"].ToString();
					Monkey monkey;
					monkeyDictionary.TryGetValue(name, out monkey);
					if(monkey == null)
					{
						monkey = result.ToMonkey();
						monkeyDictionary.Add(name,monkey);
					}
					else
					{
						monkey.Update(result);
					}
				}
				Monkeys = monkeyDictionary.Select(x=> x.Value).ToArray();
				store.BeginInvokeOnMainThread(()=>{
					if(MonkeysUpdated != null)
						MonkeysUpdated(this,EventArgs.Empty);
				});
			});
			return task;
		}

		void populateMonkeys ()
		{
			Monkeys = Monkey.GetAllMonkeys ();
			var table = store.GetTable ("monkeys");
			foreach (var monkey in Monkeys) {
				table.Insert (monkey.ToDictionary ());
			}
			DBError error = null;
			store.Sync (error);
			Console.WriteLine (error);

		}
		public void Update(Monkey monkey)
		{
			DBRecord record;
			records.TryGetValue (monkey.Name, out record);
			record.Update (monkey.ToDictionary ());
			store.SyncAsync (null);
		}

		public Monkey[] Monkeys { get; set; }
	}

	public static class DropboxHelper
	{
		public static NSDictionary ToDictionary (this Monkey monkey)
		{
			var keys = new NSString[] {
				new NSString("Name"),
				new NSString("Rotation"),
				new NSString("Scale"),
				new NSString ("X"),
				new NSString ("Y"),
				new NSString ("Z"),
			};
			var values = new NSString[] {
				new NSString(monkey.Name),
				new NSString(monkey.Rotation.ToString()),
				new NSString(monkey.Scale.ToString()),
				new NSString(monkey.X.ToString()),
				new NSString(monkey.Y.ToString()),
				new NSString(monkey.Z.ToString()),
			};
			return NSDictionary.FromObjectsAndKeys(values,keys);
		}

		public static Monkey ToMonkey (this DBRecord record)
		{
			return new Monkey ().Update (record);
			
		}

		public static Monkey Update( this Monkey monkey, DBRecord record)
		{
			monkey.Name = record.Fields [new NSString ("Name")].ToString ();
			monkey.Rotation = float.Parse (record.Fields [new NSString ("Rotation")].ToString ());
			monkey.Scale = float.Parse (record.Fields [new NSString ("Scale")].ToString ());
			monkey.X = float.Parse (record.Fields [new NSString ("X")].ToString ());
			monkey.Y = float.Parse (record.Fields [new NSString ("Y")].ToString ());
			monkey.Z = int.Parse (record.Fields [new NSString ("Z")].ToString ());
			return monkey;
		}
	}
}

