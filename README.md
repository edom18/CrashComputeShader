# このプロジェクトはなに？

Compute Shaderを編集してEditorを再生するとクラッシュする、という問題が発生しており、それを再現するプロジェクトです。

# 現象発生手順

1. AndroidにSwitch Platformする
2. `Assets/Scenes/SampleScene.unity` を開く
3. Unityを再生してクラッシュしないことを確認する（パーティクルが再生されることを確認する）
4. [ComputeShader](https://github.com/edom18/CrashComputeShader/blob/main/Assets/ComputeShaders/ParticleSystem.compute)を編集する
5. Unityを再生する

編集内容は問わず、ハードコードしているパラメータを調整するだけでも発生します。