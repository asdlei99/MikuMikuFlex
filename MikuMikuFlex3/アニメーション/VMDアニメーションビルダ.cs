﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MikuMikuFlex3
{
    public static class VMDアニメーションビルダ
    {
        /// <summary>
        ///     VMDのボーンフレームリストからアニメ変数を構築する。
        /// </summary>
        /// <param name="VMDFボーンフレームリスト">入力となるボーンフレームリスト。</param>
        /// <param name="PMXモデル">対象となるPMXモデル。</param>
        public static void ボーンモーションを追加する( VMDFormat.ボーンフレームリスト VMDFボーンフレームリスト, PMXモデル PMXモデル, bool 全ての親を無視する )
        {
            // フレーム数をボーンごとにカウントする関係で、ボーンフレーム単位ではなく、PMXボーン単位で構築する。

            // すべてのPMXボーンについて……
            for( int i = 0; i < PMXモデル.PMXボーン制御リスト.Length; i++ )
            {
                var pmxBone = PMXモデル.PMXボーン制御リスト[ i ];

                if( 全ての親を無視する && pmxBone.PMXFボーン.ボーン名 == "全ての親" )
                    continue;
                
                // 同じボーン名のフレームを列挙する。

                var boneFrames = VMDFボーンフレームリスト
                    .Where( ( frame ) => ( frame.ボーン名 == pmxBone.PMXFボーン.ボーン名 ) )  // 同じボーン名のフレームを、
                    .OrderBy( ( frame ) => frame.フレーム番号 );                              // フレーム番号昇順に。


                // 検出されたすべてのフレームについて……

                uint 前のフレーム番号 = 0;

                foreach( var frame in boneFrames )
                {
                    var 持続時間sec = ( frame.フレーム番号 - 前のフレーム番号 ) / 30.0;   // 1frame = 1/30sec

                    pmxBone.アニメ変数_移動.遷移を追加する( 
                        new アニメ遷移_移動ベジェ( frame.ボーンの位置, 持続時間sec, frame.ベジェ曲線[ 0 ], frame.ベジェ曲線[ 1 ], frame.ベジェ曲線[ 2 ] ) );

                    pmxBone.アニメ変数_回転.遷移を追加する( 
                        new アニメ遷移_回転ベジェ( frame.ボーンの回転, 持続時間sec, frame.ベジェ曲線[ 3 ] ) );

                    前のフレーム番号 = frame.フレーム番号;
                }
            }
        }
    }
}
