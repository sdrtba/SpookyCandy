#if UNITY_EDITOR
namespace YG.Insides
{
    public static partial class Langs
    {
#if RU_YG2
        public const string projectSettings = "��������� �������";
        public const string t_nameDefining = "��� ��������� - ������������ ��� Scripting Define Symbols � ��� ������ ������������ ��������� ���������.";
        public const string namePlatform = "��� ���������";
        public const string applySettingsProject = "��������� ��������� �������";
        public const string addProjectSettings = "�������������� �����";
#else
        public const string projectSettings = "Progect settings";
        public const string t_nameDefining = "Platform name - used for Scripting Define Symbols and for the class implementing the platform interface.";
        public const string namePlatform = "Name platform";
        public const string applySettingsProject = "Apply settings project";
        public const string addProjectSettings = "Additional options";
#endif
    }
}
#endif