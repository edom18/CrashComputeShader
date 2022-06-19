# What's this project?

This project reproduces a crash problem with a Compute Shader in Unity.

# How to reproduce

1. Switch project to Android
2. Open `Assets/Scenes/SampleScene.unity` scene
3. Make sure that it DOES NOT crash when playing the scene
4. Edit the [compute shader](https://github.com/edom18/CrashComputeShader/blob/main/Assets/ComputeShaders/ParticleSystem.compute)
5. Play the editor again

-------------------------------------------------

# このプロジェクトはなに？

Compute Shaderを編集してEditorを再生するとクラッシュする、という問題が発生しており、それを再現するプロジェクトです。

# 現象発生手順

1. AndroidにSwitch Platformする
2. `Assets/Scenes/SampleScene.unity` を開く
3. Unityを再生してクラッシュしないことを確認する（パーティクルが再生されることを確認する）
4. [ComputeShader](https://github.com/edom18/CrashComputeShader/blob/main/Assets/ComputeShaders/ParticleSystem.compute)を編集する
5. Unityを再生する

編集内容は問わず、ハードコードしているパラメータを調整するだけでも発生します。