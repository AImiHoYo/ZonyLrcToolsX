﻿using Shouldly;
using System.Threading.Tasks;
using Xunit;
using ZonyLrcToolsX.Downloader.Lyric.Exceptions;
using ZonyLrcToolsX.Downloader.Lyric.NetEase;
using ZonyLrcToolsX.Downloader.Lyric.QQMusic;
using ZonyLrcToolsX.Infrastructure.Configuration;
using ZonyLrcToolsX.Infrastructure.MusicTag;

namespace ZonyLrcToolsX.Tests.Downloader.LyricDownloader
{
    public class NetEaseCloudMusicLyricDownloader_Tests
    {
        [Fact]
        public async Task DownloadAsync_Original_Lyric_Test()
        {
            // Arrange
            AppConfiguration.Instance.Configuration.LyricContentType = LyricContentTypes.Original;
            
            // Act & Assert
            await Internal_DownloadAsync_Test();
        }

        [Fact]
        public async Task DownloadAsync_Translation_Lyric_Test()
        {
            // Arrange
            AppConfiguration.Instance.Configuration.LyricContentType = LyricContentTypes.Translation;
            
            // Act & Assert
            await Internal_DownloadAsync_Test();
        }

        [Fact]
        public async Task DownloadAsync_Original_And_Translation_Lyric_Test()
        {
            // Arrange
            AppConfiguration.Instance.Configuration.LyricContentType = LyricContentTypes.OriginalAndTranslation;
            
            // Act & Assert
            await Internal_DownloadAsync_Test();
        }

        private async Task Internal_DownloadAsync_Test()
        {
            var downloader = new NetEaseCloudMusicLyricDownloader();
            
            // 仅包含原始歌词。
            var musicInfo1 = new MusicInfo("黄金甲","周杰伦");
            
            // 包含原始 + 翻译歌词。
            var musicInfo2 = new MusicInfo("あさき、ゆめみし", "瀧沢一留");
            
            // 纯音乐歌词。
            var musicInfo3 = new MusicInfo("Masked Heroes", "Vexento");
            
            // 不存在的歌曲。
            var musicInfo4 = new MusicInfo("AAAAAAAAAAADDDDFF", "ASDASDF");

            // 数据为 NULL。
            var musicInfo5 = new MusicInfo(null, null);

            // 空字符串。
            var musicInfo6 = new MusicInfo("", "");

            // Act
            var result1 = await downloader.DownloadAsync(musicInfo1);
            var result2 = await downloader.DownloadAsync(musicInfo2);
            var result3 = await downloader.DownloadAsync(musicInfo3);

            // Assert
            result1.ShouldNotBeNull();
            if (AppConfiguration.Instance.Configuration.LyricContentType == LyricContentTypes.Translation)
            {
                result1.IsPureMusic.ShouldBe(true);
                result1.Count.ShouldBe(0);
            }
            else
            {
                result1.IsPureMusic.ShouldBe(false);
            }
            
            result2.ShouldNotBeNull();
            result2.IsPureMusic.ShouldBe(false);
            result2.Count.ShouldBeGreaterThan(1);
            
            result3.ShouldNotBeNull();
            result3.IsPureMusic.ShouldBe(true);
            result3.Count.ShouldBe(0);
            
            await Should.ThrowAsync<NotFoundSongException>(async () => await downloader.DownloadAsync(musicInfo4));
            await Should.ThrowAsync<NotFoundSongException>(async () => await downloader.DownloadAsync(musicInfo5));
            await Should.ThrowAsync<NotFoundSongException>(async () => await downloader.DownloadAsync(musicInfo6));
        }

        [Fact]
        public async Task SongCount_Is_Zero_Should_Return_A_LyricCollectItem()
        {
            var downloader = new NetEaseCloudMusicLyricDownloader();
            var lyricA = await downloader.DownloadAsync(new MusicInfo("MOIL","須田景凪"));
            var lyricB = await downloader.DownloadAsync(new MusicInfo("couch", "須田景凪"));
            
            lyricA.ShouldNotBeNull();
            lyricA.IsPureMusic.ShouldBe(false);
            
            lyricB.ShouldNotBeNull();
            lyricB.IsPureMusic.ShouldBe(false);
        }
    }
}