/* Copyright (c) Citrix Systems, Inc. 
 * All rights reserved. 
 * 
 * Redistribution and use in source and binary forms, 
 * with or without modification, are permitted provided 
 * that the following conditions are met: 
 * 
 * *   Redistributions of source code must retain the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer. 
 * *   Redistributions in binary form must reproduce the above 
 *     copyright notice, this list of conditions and the 
 *     following disclaimer in the documentation and/or other 
 *     materials provided with the distribution. 
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND 
 * CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, 
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF 
 * MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE 
 * DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, 
 * SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, 
 * BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR 
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
 * WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING 
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE 
 * OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF 
 * SUCH DAMAGE.
 */

using System.Collections.Generic;

namespace XenAdmin.Controls.CheckableDataGridView
{
    public interface ICheckableDataGridViewView
    {
        CheckableDataGridViewController Controller { set; }
        void DrawRow(CheckableDataGridViewRow row);
        void DrawRowAsChecked(bool checkStatus, int rowIndex);
        void DrawRowAsHighlighted(bool highlightStatus, int rowIndex);
        void DrawRowAsDisabled(bool disabledStatus, int rowIndex);
        void TriggerRowUpdatedEvent(int rowUpdated, bool refreshGrid);
        void TriggerRowCheckedEvent(int rowChecked);
        void DrawUpdatedRow(Queue<object> textToUse, bool cellDataLoaded, bool rowDisabled, int rowIndex);
        void DrawAllRowsAsCleared();
        void DrawAllRowsAsClearedMW();
        void DrawRowMW(CheckableDataGridViewRow row);
        void DrawRowAsHighlightedMW(bool highlightStatus, int rowIndex);
        void DrawSetRowInformation(int rowIndex, string information);
        void DrawRowAsLocked(bool lockStatus, int rowIndex);
        void DrawColumnGlyph(CheckableDataGridViewController.SortDirection sortDirection, int columnNumber);
        void SuspendDrawing();
        void ResumeDrawing();    }
}
