## Unity Toys
(2021. 01. 18 ~)

#

# AfterImage (MotionTrail)
- 사용법
  - 대상이 Mesh인 경우 MeshAfterImage 스크립트를, SkinnedMesh인 경우 SkinnedMeshAfterImage 스크립트를 대상 게임오브젝트에 넣는다.
  - 잔상 효과로 사용할 마테리얼을 AfterImage Material에 넣는다.
  - AfterImage Gradient를 설정하여 변화할 색상 그라데이션을 지정할 수 있다.
  - 자식 게임오브젝트들의 모든 메시/스킨메시도 같이 복제하려면 ContainChildrenMeshes를 체크한다.
  
- 구현 원리
  - 일정 간격으로 메시(스킨메시의 경우에는 베이크한 메시), 트랜스폼 정보를 잔상 게임오브젝트들에 전달한다.
  - 각각의 잔상 게임오브젝트들은 정보를 전달받은 순간부터 일정 간격으로 마테리얼의 알파값을 감소시킨다.
  - 잔상 생성과 파괴는 생성 및 파괴 대신 활성 큐/대기 큐를 이용한 풀링 기법을 사용한다.
  
## [1] 메시

<img src="https://user-images.githubusercontent.com/42164422/104916405-6017a300-59d5-11eb-8527-f6090e3465d9.png" width="500">
<img src="https://user-images.githubusercontent.com/42164422/104916486-7c1b4480-59d5-11eb-9aa2-5ad96490d932.gif" width="500">

## [2] 스킨메시

<img src="https://user-images.githubusercontent.com/42164422/104916473-76bdfa00-59d5-11eb-98b1-bcedfc89eb63.png" width="500">
<img src="https://user-images.githubusercontent.com/42164422/104916494-7e7d9e80-59d5-11eb-9bff-71be140535ea.gif" width="500">

#

# Mesh Generators
## [1] Regular Polygon
<img src="https://user-images.githubusercontent.com/42164422/104811800-62e18f00-5841-11eb-8fa5-2a82d7e79616.png" width="500">

## [2] Prism
<img src="https://user-images.githubusercontent.com/42164422/104811801-64ab5280-5841-11eb-9b81-da5d5dac9ed8.png" width="500">

<img src="https://user-images.githubusercontent.com/42164422/104811385-6b849600-583e-11eb-8ecb-ef1ba4a09ae8.png" width="500">

## [3] Perlin Noise Ground
<img src="https://user-images.githubusercontent.com/42164422/104811803-683ed980-5841-11eb-93c7-d20f6ba2b7a9.png" width="500">

<img src="https://user-images.githubusercontent.com/42164422/104811400-8b1bbe80-583e-11eb-9416-c53a38faec41.png" width="500">
<img src="https://user-images.githubusercontent.com/42164422/104811396-7f2ffc80-583e-11eb-9a0b-61a0cbb4c942.png" width="500">

## [4] Snow Ground
<img src="https://user-images.githubusercontent.com/42164422/104811804-69700680-5841-11eb-81d1-85024339782e.png" width="500">

<img src="https://user-images.githubusercontent.com/42164422/104811363-3e37e800-583e-11eb-916f-ea45abb74c41.gif" width="500">
<img src="https://user-images.githubusercontent.com/42164422/104811365-409a4200-583e-11eb-9d2e-5a34e10a1130.gif" width="500">

#

# Field of View
<img src="https://user-images.githubusercontent.com/42164422/104811466-02e9e900-583f-11eb-84b4-18568724271a.gif" width="500">

<img src="https://user-images.githubusercontent.com/42164422/104811469-05e4d980-583f-11eb-85b2-f1998fb35ec3.gif" width="500">

- Reference
  - https://www.youtube.com/watch?v=rQG9aUWarwE [Sebastian Lague]

#

# Pixelater

<img src="https://user-images.githubusercontent.com/42164422/105009217-90b31780-5a7d-11eb-8feb-bf1062c91286.gif" width="500">

- 렌더 텍스쳐의 해상도를 강제로 변경하여 픽셀화시킨다.
- 카메라에 스크립트를 부착하여 사용

- Reference
  - https://www.youtube.com/watch?v=5rMkh9sl2bM [DH Studio]
  
#

# Frame Rate Checker

<img src="https://user-images.githubusercontent.com/42164422/105624478-6d191400-5e65-11eb-89ba-5a894dcfdce3.gif" width="500">

