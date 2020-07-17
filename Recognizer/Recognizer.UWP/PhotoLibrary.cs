﻿using Recognizer.UWP;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Xamarin.Forms;

[assembly: Dependency(typeof(PhotoLibrary))]
namespace Recognizer.UWP
{
    public class PhotoLibrary : IPhotoLibrary
    {
        public async Task<Stream> PickPhotoAsync()
        {
            // Create and initialize the FileOpenPicker
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary,
            };

            openPicker.FileTypeFilter.Add(".jpg");
            openPicker.FileTypeFilter.Add(".jpeg");
            openPicker.FileTypeFilter.Add(".png");

            // Get a file and return a Stream 
            StorageFile storageFile = await openPicker.PickSingleFileAsync();

            if (storageFile == null)
            {
                return null;
            }

            IRandomAccessStreamWithContentType raStream = await storageFile.OpenReadAsync();
            return raStream.AsStreamForRead();
        }

        public async Task<bool> SavePhotoAsync(byte[] data, string folder, string filename)
        {
            StorageFolder picturesDirectory = KnownFolders.PicturesLibrary;
            StorageFolder folderDirectory = picturesDirectory;

            // Get the folder or create it if necessary

            try
            {
                // Create the file.
                StorageFile storageFile = await folderDirectory.CreateFileAsync(filename,
                                                    CreationCollisionOption.ReplaceExisting);

                // Convert byte[] to Windows buffer and write it out.
                IBuffer buffer = WindowsRuntimeBuffer.Create(data, 0, data.Length, data.Length);
                await FileIO.WriteBufferAsync(storageFile, buffer);
            }
            catch
            {
                return false;
            }

            return true;
        }

        public async Task<bool> SaveJsonAsync(string data, string folder, string filename)
        {
            StorageFolder picturesDirectory = KnownFolders.PicturesLibrary;
            StorageFolder folderDirectory = picturesDirectory;

            // Get the folder or create it if necessary
           
            
            try
            {
                // Create the file.
                StorageFile storageFile = await folderDirectory.CreateFileAsync(filename,
                                                    CreationCollisionOption.GenerateUniqueName);
                // Convert byte[] to Windows buffer and write it out.
                await FileIO.WriteTextAsync(storageFile, data);
            }
            catch
            {
                return false;
            }

            return true;
        }

    }
}
