using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CodeFlix.Data;
using CodeFlix.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace CodeFlix.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EpisodesController : ControllerBase
    {
        private readonly CodeFlixContext _context;

        public EpisodesController(CodeFlixContext context)
        {
            _context = context;
        }

        // GET: api/Episodes
        [HttpGet]
        [Authorize]
        public ActionResult<List<Episode>> GetEpisodes(int mediaId, byte seasonNumber)
        {
            return _context.Episodes.Where(e => e.MediaId == mediaId && e.SeasonNumber == seasonNumber).OrderBy(e => e.EpisodeNumber).AsNoTracking().ToList();
        }

        // GET: api/Episodes/5
        [HttpGet("{id}")]
        [Authorize]
        public ActionResult<Episode> GetEpisode(int mediaId, byte seasonNumber, short episodeNumber)
        {
            var result = _context.Episodes.Where(e => e.MediaId == mediaId && e.SeasonNumber == seasonNumber && e.EpisodeNumber == episodeNumber).FirstOrDefault();

            if (result != null)
            {
                return result;
            }
            return NotFound("Episode not found");
        }

        [HttpGet("Watch")]
        [Authorize]
        public void Watch(long id)//bu metot oynatmak için değil izlendiği anda sayısını bir arttırıp kayıt altına almak için var
        {
            UserWatched userWatched = new UserWatched();
            Episode episode = _context.Episodes.Find(id)!;

            try
            {
                userWatched.UserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                userWatched.EpisodeId = id;
                _context.UserWatcheds.Add(userWatched);
                episode.ViewCount++;
                _context.Episodes.Update(episode);
                _context.SaveChanges();
                //ilk izlemede artar
            }
            catch (Exception e)
            {
                throw;
            }
            //her izlemede artar
            episode.ViewCount++;
            _context.Episodes.Update(episode); 
            _context.SaveChanges();
        }

        // PUT: api/Episodes/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator, ContentAdmin")]
        public async Task<IActionResult> PutEpisode(long id, Episode episode)
        {
            Episode? episodeToUpdate = null;
            if (id != episode.Id)
            {
                return BadRequest();
            }

            episodeToUpdate = _context.Episodes.Where(e => e.Id == episode.Id).FirstOrDefault();
            if (episodeToUpdate == null)
            {
                return NotFound("Episode to update not found.");
            }
            episodeToUpdate.Title = episode.Title;
            episodeToUpdate.Description = episode.Description;
            episodeToUpdate.SeasonNumber = episode.SeasonNumber;
            episodeToUpdate.EpisodeNumber = episode.EpisodeNumber;
            episodeToUpdate.Duration = episode.Duration;
            episodeToUpdate.Passive = episode.Passive;

            //_context.Episodes.Update(episode);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/Episodes
        [HttpPost]
        [Authorize(Roles = "Administrator, ContentAdmin")]
        public async Task<ActionResult<Episode>> PostEpisode(Episode episode)
        {
            episode.ViewCount = 0;

            // Episode'nin MediaId'sine göre ilgili Media nesnesini bulun
            var media = await _context.Medias.FindAsync(episode.MediaId);

            // Eğer belirtilen MediaId'ye sahip bir medya bulunamazsa BadRequest döndürün
            if (media == null)
            {
                return BadRequest("Belirtilen MediaId'ye sahip bir medya bulunamadı.");
            }

            // Episode'nin Media özelliğine ilgili Media nesnesini ata
            episode.Media = media;

            // Episode'yi veritabanına ekleyin
            _context.Episodes.Add(episode);
            await _context.SaveChangesAsync();

            return Ok(episode);
        }

        // DELETE: api/Episodes/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator, ContentAdmin")]
        public ActionResult DeleteEpisode(long id)
        {
            var episode = _context.Episodes.FindAsync(id).Result;
            if (episode == null)
            {
                return NotFound();
            }
            episode.Passive = true;
            _context.Episodes.Update(episode);
            _context.SaveChangesAsync();

            return Ok("Episode deleted.");
        }
    }
}
