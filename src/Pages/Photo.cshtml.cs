﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PhotoGallery.Models;
using System;
using System.IO;
using System.Linq;

namespace PhotoGallery.Pages
{
    public class PhotoModel : PageModel
    {
        private AlbumCollection _ac;
        private IHostingEnvironment _environment;

        public PhotoModel(AlbumCollection ac, IHostingEnvironment environment)
        {
            _ac = ac;
            _environment = environment;
        }

        public Photo Photo { get; set; }

        public void OnGet(string albumName, string photoName)
        {
            var album = _ac.Albums.FirstOrDefault(a => a.Name.Equals(albumName, StringComparison.OrdinalIgnoreCase));
            Photo = album.Photos.FirstOrDefault(p => p.DisplayName.Equals(photoName, StringComparison.OrdinalIgnoreCase));
            ViewData["Title"] = $"{Photo.DisplayName} - {Photo.Album.Name}";
        }

        public IActionResult OnPostRename(string albumName, string photoName)
        {
            var album = _ac.Albums.FirstOrDefault(a => a.Name.Equals(albumName, StringComparison.OrdinalIgnoreCase));
            Photo = album.Photos.FirstOrDefault(p => p.DisplayName.Equals(photoName, StringComparison.OrdinalIgnoreCase));
            string name = Request.Form["name"] + Path.GetExtension(Photo.AbsolutePath);

            var newPhotoPath = new FileInfo(Path.Combine(album.AbsolutePath, name));
            int index = album.Photos.IndexOf(Photo);

            System.IO.File.Copy(Photo.AbsolutePath, newPhotoPath.FullName, true);
            var newPhoto = new Photo(album, newPhotoPath);

            album.Photos.Insert(index, newPhoto);
            album.Photos.RemoveAt(index + 1);

            if (System.IO.File.Exists(Photo.AbsolutePath))
            {
                System.IO.File.Delete(Photo.AbsolutePath);
                string folder = Path.Combine(album.AbsolutePath, "thumbnail");
                var pattern = $"{Photo.DisplayName}-*x*{Path.GetExtension(Photo.AbsolutePath)}";

                foreach (var file in Directory.EnumerateFiles(folder, pattern))
                {
                    System.IO.File.Delete(file);
                }
            }

            return new RedirectResult($"~/photo/{albumName}/{newPhoto.DisplayName}/");

        }

        public IActionResult OnPostDelete(string albumName, string photoName)
        {
            var album = _ac.Albums.FirstOrDefault(a => a.Name.Equals(albumName, StringComparison.OrdinalIgnoreCase));
            Photo = album.Photos.FirstOrDefault(p => p.DisplayName.Equals(photoName, StringComparison.OrdinalIgnoreCase));
            album.Photos.Remove(Photo);

            if (System.IO.File.Exists(Photo.AbsolutePath))
            {
                System.IO.File.Delete(Photo.AbsolutePath);
                string folder = Path.Combine(album.AbsolutePath, "thumbnail");
                var pattern = $"{Photo.DisplayName}-*x*{Path.GetExtension(Photo.AbsolutePath)}";

                foreach (var file in Directory.EnumerateFiles(folder, pattern))
                {
                    System.IO.File.Delete(file);
                }
            }

            return new RedirectResult($"~/albums/{albumName}/");
        }
    }
}
