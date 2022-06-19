# このプロジェクトはなに？

Compute Shaderを編集してEditorを再生するとクラッシュする、という問題が発生しており、それを再現するプロジェクトです。

# 現象発生手順

1. AndroidにSwitch Platformする
2. [ComputeShader](https://github.com/edom18/CrashComputeShader/blob/main/Assets/ComputeShaders/ParticleSystem.compute)を編集する
3. Unityを再生する

編集内容は問わず、ハードコードしているパラメータを調整するだけでも発生します。