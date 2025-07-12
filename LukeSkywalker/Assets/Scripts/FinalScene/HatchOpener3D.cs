using UnityEngine;
using DG.Tweening;

public class HatchOpener3D : MonoBehaviour {
  [Header("�������")]
  public Transform pivot;        // ����������� ��������� (��� �����)
  public Transform frontSide;    // �������� ������� (Quad)
  public Transform backSide;     // ������ ������� (Quad, ���������)
  public Transform blackCircle;  // ׸���� ���� ������
  public Camera mainCamera;      // �������� ������

  [Header("���������")]
  public float thickness = 0.1f;   // ������� ����
  public float openAngle = 160f;   // ���� ��������
  public float openDuration = 2f;  // ������������ ��������
  public float zoomDuration = 2f;  // ������������ �����������

  private void Start() {
    SetupInitial();
    AnimateHatch();
  }

  private void SetupInitial() {
    // ��������� �������
    frontSide.localPosition = new Vector3(0, 0, -thickness / 2f);
    backSide.localPosition = new Vector3(0, 0, thickness / 2f);

    // ��������� ������ �������
    backSide.localRotation = Quaternion.Euler(0f, 180f, 0f);

    // ׸���� ���� ������, �� ��� �� ����� ����
    blackCircle.SetAsFirstSibling();
  }

  private void AnimateHatch() {
    // ������� �������� ����
    pivot.DOLocalRotate(new Vector3(0f, openAngle, 0f), openDuration)
        .SetEase(Ease.OutBack)
        .OnComplete(() => ZoomToBlack());
  }

  private void ZoomToBlack() {
    Vector3 targetPos = blackCircle.position;
    targetPos.z = mainCamera.transform.position.z;

    // ������������ ���� �� ��� ���, ���� �� �� ��������� �����
    mainCamera.transform.DOMove(targetPos, zoomDuration);
    mainCamera.DOOrthoSize(0.01f, zoomDuration);  // ��� ����� FOV, ���� ������ �������������
  }
}
