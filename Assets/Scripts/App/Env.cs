namespace App
{
	public class Env
	{
		public enum Region {
			JAPAN,
			WORLDWIDE
		}

		public enum Language {
			JAPANESE,
			ENGLISH,
			CHINESE_ZHJ,
			CHINESE_ZH,
			KOREAN,
			FRENCH,
			GERMAN,
			SPANISH,
			PORTUGUESE,
			ITALIAN,
			RUSSIAN,
			//DUTCH
		}

#if DEBUG || UNITY_EDITOR
		public enum ActionServerType {
			LOCAL1,
			LOCAL2,
			LOCAL3,
			LOCAL4,
			MTBTEST,
			FP137TEST1,
			FP137TEST2,
			LEADERBOARDTEST,
			STAGING,
			RELEASE,
			APPLICATION
		}
#else
		public enum ActionServerType {
			LOCAL1,
			LOCAL2,
			LOCAL3,
			LOCAL4,
			LOCAL5,
			LOCAL6,
			LOCAL7,
			LEADERBOARDTEST,
			LOCAL9,
			RELEASE,
			APPLICATION
		}
#endif

#if DEBUG || UNITY_EDITOR
		public const bool isDebug = true;
#else
		public const bool isDebug = false;
#endif

		public const bool isDebugFont = false;

		public const bool forDevelop = false;

#if UNITY_EDITOR
		public static bool m_useAssetBundle = false;
#else
		public static bool m_useAssetBundle = true;
#endif

		private static readonly bool m_releaseApplication;

		private static Region mRegion;

		private static Language mLanguage;

#if DEBUG || UNITY_EDITOR
		private static ActionServerType mActionServerType = ActionServerType.RELEASE;
#else
		private static ActionServerType mActionServerType = ActionServerType.RELEASE;
#endif

		public static bool useAssetBundle {
			get { return m_useAssetBundle; }
		}

		public static bool isReleaseApplication {
			get { return m_releaseApplication; }
		}

		public static Region region {
			get { return mRegion; }
			set { mRegion = value; }
		}

		public static Language language {
			get { return mLanguage; }
			set { mLanguage = value; }
		}

		public static ActionServerType actionServerType {
			get { return mActionServerType; }
			set { mActionServerType = value; }
		}
	}
}
