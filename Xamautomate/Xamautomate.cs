using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace davidnicholai.Xamautomate
{
    public class Xamautomate
    {
        private Dictionary<string, Func<AppQuery, AppQuery>> _queries;
        private IApp _app;

        public Xamautomate(Platform platform, string bundlePath = "")
        {
            _app = configureApp(platform, bundlePath);
            _queries = new Dictionary<string, Func<AppQuery, AppQuery>>();
        }

        #region Private Methods

        private Func<AppQuery, AppQuery> getQueryFromKey(string key)
        {
            try
            {
                return _queries.FirstOrDefault(x => x.Key.Equals(key)).Value;
            }
            catch
            {
                throw new ArgumentNullException(key);
            }
        }

        private IApp configureApp(Platform platform, string bundlePath = "")
        {
            if (platform.Equals(Platform.Android))
            {
                return string.IsNullOrEmpty(bundlePath) ?
                    ConfigureApp.Android.StartApp() :
                    ConfigureApp.Android.ApkFile(bundlePath).StartApp();
            }

            return string.IsNullOrEmpty(bundlePath) ?
                ConfigureApp.iOS.StartApp() :
                ConfigureApp.iOS.AppBundle(bundlePath).StartApp();
        }

        private void keyExists(string key)
        {
            if (_queries.ContainsKey(key))
            {
                throw new ArgumentException();
            }
        }

        #endregion

        public void AddQuery(string key, Func<AppQuery, AppQuery> query)
        {
            keyExists(key);
            _queries.Add(key, query);
        }

        public void AddQueryFromAutomationId(string key, string automationId)
        {
            keyExists(key);
            _queries.Add(key, e => e.Marked(automationId));
        }

        public void AddQueryFromButtonText(string key, string buttonText)
        {
            keyExists(key);
            _queries.Add(key, e => e.Text(buttonText));
        }

        public void Tap(string key)
        {
            _app.Tap(getQueryFromKey(key));
        }

        public void WaitAndTap(string key)
        {
            var query = getQueryFromKey(key);

            _app.WaitForElement(query);
            _app.Tap(query);
        }

        /// <summary>
        /// Doesn't matter if the View can't be found.
        /// </summary>
        /// <param name="key">Key.</param>
        public void WaitAndTapOptional(string key)
        {
            var query = getQueryFromKey(key);

            try
            {
                _app.WaitForElement(query);
                _app.Tap(query);
            }
            catch { }
        }

        public void Back()
        {
            _app.Back();
        }

        public void ClearText()
        {
            _app.ClearText();
        }

        public void PressEnter()
        {
            _app.PressEnter();
        }

        public void EnterText(string text)
        {
            _app.EnterText(text);
        }

        public void EnterText(string key, string text)
        {
            _app.EnterText(getQueryFromKey(key), text);
        }

        public void ScrollDownTo(string keyToTap, string keyToScroll, ScrollStrategy strategy = ScrollStrategy.Auto, double swipePercentage = 0.67, int swipeSpeed = 500, bool withInertia = true, TimeSpan? timeout = default(TimeSpan?))
        {
            _app.ScrollDownTo(getQueryFromKey(keyToTap), getQueryFromKey(keyToScroll), strategy, swipePercentage, swipeSpeed, withInertia, timeout);
        }
    }
}
