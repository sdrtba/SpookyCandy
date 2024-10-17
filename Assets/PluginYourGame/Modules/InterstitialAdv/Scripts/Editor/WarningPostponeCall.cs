using UnityEngine;

namespace YG.EditorScr
{
    public static class WarningPostponeCall
    {
        public static bool Draw()
        {
            Debug.Log(YG2.infoYG.InterstitialAdv.postponeCallByFail);
            if (!YG2.infoYG.InterstitialAdv.postponeCallByFail)
            {
                GUILayout.Space(10);
#if RU_YG2
                string labelText = "��� ���������� ������ ������� ���������� �������� ����� �Postpone Call By Fail� � ����������";
                string buttonText = "�������� ����� �Postpone Call By Fail�";
#else
                string labelText = "For the script to work correctly, you need to enable the �Postpone Call By Fail� option in the settings";
                string buttonText = "Enable the �Postpone Call By Fail� option";
#endif
                GUILayout.Label($"\n{labelText} {InfoYG.NAME_PLUGIN}!\n", YGEditorStyles.error);

                if (FastButton.Standart(buttonText))
                    YG2.infoYG.InterstitialAdv.postponeCallByFail = true;

                return true;
            }
            else if (YG2.infoYG.InterstitialAdv.postponeCallTimer < 10)
            {
                GUILayout.Space(10);
#if RU_YG2
                string labelText1 = "�������� �������� ���������� ������ ������� �Postpone Call Timer� � ����������";
                string labelText2 = "������������� ���������� �� ����� 10-�� ������!";
                string buttonText = "��������� �������� �Postpone Call Timer� �� 10 ������";
#else
                string labelText1 = "The delay parameter for the next ad call is �Postpone Call Timer� in the settings";
                string labelText2 = "It is recommended to set at least 10 seconds!";
                string buttonText = "Set the �Postpone Call Timer� parameter for 10 seconds";
#endif
                GUILayout.Label($"\n{labelText1} {InfoYG.NAME_PLUGIN} - {labelText2}\n", YGEditorStyles.warning);

                if (FastButton.Standart(buttonText))
                    YG2.infoYG.InterstitialAdv.postponeCallTimer = 10;

                return true;
            }

            return false;
        }
    }
}
