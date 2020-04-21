using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using Notie.Models;
using Xamarin.Forms;

namespace Notie
{
    public class DBRepository
    {
        SQLiteConnection database;

        public DBRepository(string filename)
        {
            string databasePath = DependencyService.Get<ISQLite>().GetDatabasePath(filename);
            database = new SQLiteConnection(databasePath);
            database.CreateTable<Note>();
            database.CreateTable<Folder>();
        }
        public IEnumerable<Note> GetItems()
        {
            return (from i in database.Table<Note>() where i.Folder_Id == 0 select i).ToList();
        }

        public Note GetItem(int id)
        {
            return database.Get<Note>(id);
        }

        public int DeleteItem(int id)
        {
            return database.Delete<Note>(id);
        }

        public int SaveItem(Note item)
        {
            if (item.Id != 0)
            {
                return database.Update(item);
            }
            return database.Insert(item);
        }

        public IEnumerable<Folder> GetFolders()
        {
            return database.Table<Folder>().ToList();
        }

        public Folder GetFolder(int id)
        {
            try
            {
                return database.Get<Folder>(id);
            }
            catch
            {
                return null;
            }

        }

        public Folder GetFolderByName(string name)
        {
            return database.Table<Folder>().FirstOrDefault(folder => folder.Name == name);
        }

        public int DeleteFolder(int id)
        {
            database.Query<Note>("UPDATE NOTE SET FOLDER_ID = 0 WHERE FOLDER_ID = " + id);
  
            return database.Delete<Folder>(id);
        }

        public int SaveFolder(Folder f)
        {
            if (f.Id != 0)
            {
                return database.Update(f);
            }

            return database.Insert(f);
        }

        public IEnumerable<Note> GetItemsInFolder(Folder folder)
        {
            return (from note in database.Table<Note>() 
            where note.Folder_Id == folder.Id select note).ToList();
        }

        public override string ToString()
        {
            return database.DatabasePath;
        }
    }
}
