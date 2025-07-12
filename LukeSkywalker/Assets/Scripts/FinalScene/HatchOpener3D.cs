using UnityEngine;
using DG.Tweening;

public class HatchOpener3D : MonoBehaviour {
  [Header("Объекты")]
  public Transform pivot;        // Вращающийся контейнер (ось слева)
  public Transform frontSide;    // Передняя сторона (Quad)
  public Transform backSide;     // Задняя сторона (Quad, повернута)
  public Transform blackCircle;  // Чёрный круг позади
  public Camera mainCamera;      // Основная камера

  [Header("Параметры")]
  public float thickness = 0.1f;   // Толщина люка
  public float openAngle = 160f;   // Угол открытия
  public float openDuration = 2f;  // Длительность открытия
  public float zoomDuration = 2f;  // Длительность приближения

  private void Start() {
    SetupInitial();
    AnimateHatch();
  }

  private void SetupInitial() {
    // Установим толщину
    frontSide.localPosition = new Vector3(0, 0, -thickness / 2f);
    backSide.localPosition = new Vector3(0, 0, thickness / 2f);

    // Повернуть заднюю сторону
    backSide.localRotation = Quaternion.Euler(0f, 180f, 0f);

    // Чёрный круг позади, но его не видно пока
    blackCircle.SetAsFirstSibling();
  }

  private void AnimateHatch() {
    // Плавное открытие люка
    pivot.DOLocalRotate(new Vector3(0f, openAngle, 0f), openDuration)
        .SetEase(Ease.OutBack)
        .OnComplete(() => ZoomToBlack());
  }

  private void ZoomToBlack() {
    Vector3 targetPos = blackCircle.position;
    targetPos.z = mainCamera.transform.position.z;

    // Масштабируем круг до тех пор, пока он не перекроет экран
    mainCamera.transform.DOMove(targetPos, zoomDuration);
    mainCamera.DOOrthoSize(0.01f, zoomDuration);  // Или через FOV, если камера перспективная
  }
}
